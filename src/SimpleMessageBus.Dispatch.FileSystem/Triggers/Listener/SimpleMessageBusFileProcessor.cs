// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{

    /// <summary>
    /// Default file processor used by <see cref="FileTriggerAttribute"/>.
    /// </summary>
    public class SimpleMessageBusFileProcessor
    {
        private readonly FileSystemOptions _options;
        private readonly SimpleMessageBusFileTriggerAttribute _attribute;
        private readonly string _queueFolder;
        private readonly ILogger _logger;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly JsonSerializer _serializer;
        private string _instanceId;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="context">The <see cref="SimpleMessageBusFileProcessorFactoryContext"/> to use.</param>
        public SimpleMessageBusFileProcessor(SimpleMessageBusFileProcessorFactoryContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _options = context.Options;
            _attribute = context.Attribute;
            _queueFolder = context.QueueFolder;
            _executor = context.Executor;
            _logger = context.Logger;

            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };
            _serializer = JsonSerializer.Create(settings);
        }

        /// <summary>
        /// Gets the file extension that will be used for the status files
        /// that are created for processed files.
        /// </summary>
        public virtual string StatusFileExtension => "status";

        /// <summary>
        /// Gets the maximum degree of parallelism that will be used
        /// when processing files concurrently.
        /// </summary>
        /// <remarks>
        /// Files are added to an internal processing queue as file events
        /// are detected, and they're processed in parallel based on this setting.
        /// </remarks>
        public virtual int MaxDegreeOfParallelism => 5;

        /// <summary>
        /// Gets the bounds on the maximum number of files that can be queued
        /// up for processing at one time. When set to -1, the work queue is
        /// unbounded.
        /// </summary>
        public virtual int MaxQueueSize => -1;

        /// <summary>
        /// Gets the maximum number of times file processing will
        /// be attempted for a file.
        /// </summary>
        public virtual int MaxProcessCount => 3;

        /// <summary>
        /// Gets the current role instance ID. In Azure WebApps, this will be the
        /// WEBSITE_INSTANCE_ID. In non Azure scenarios, this will default to the
        /// Process ID.
        /// </summary>
        public virtual string InstanceId
        {
            get
            {
                if (_instanceId is null)
                {
                    string envValue = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
                    if (!string.IsNullOrEmpty(envValue))
                    {
                        _instanceId = envValue.Substring(0, 20);
                    }
                    else
                    {
#if NET6_0_OR_GREATER
                        _instanceId = Environment.ProcessId.ToString();
#else
                        _instanceId = Process.GetCurrentProcess().Id.ToString();
#endif
                    }
                }
                return _instanceId;
            }
        }

        /// <summary>
        /// Process the file indicated by the specified <see cref="FileSystemEventArgs"/>.
        /// </summary>
        /// <param name="eventArgs">The <see cref="FileSystemEventArgs"/> indicating the file to process.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>
        /// A <see cref="Task"/> that returns true if the file was processed successfully, false otherwise.
        /// </returns>
        public virtual async Task<bool> ProcessFileAsync(FileSystemEventArgs eventArgs, CancellationToken cancellationToken)
        {
            try
            {
                StatusFileEntry status = null;
                string filePath = eventArgs.FullPath;
                using (var statusWriter = AcquireStatusFileLock(filePath, eventArgs.ChangeType, out status))
                {
                    if (statusWriter is null)
                    {
                        return false;
                    }

                    // We've acquired the lock. The current status might be either Failed
                    // or Processing (if processing failed before we were unable to update
                    // the file status to Failed)
                    int processCount = 0;
                    if (status is not null)
                    {
                        processCount = status.ProcessCount;
                    }

                    while (processCount++ < MaxProcessCount)
                    {
                        FunctionResult result = null;
                        if (result is not null)
                        {
                            var delay = GetRetryInterval(result, processCount);
                            await Task.Delay(delay, cancellationToken);
                        }

                        // write an entry indicating the file is being processed
                        status = new StatusFileEntry
                        {
                            State = ProcessingState.Processing,
                            Timestamp = DateTime.Now,
                            LastWrite = File.GetLastWriteTimeUtc(filePath),
                            ChangeType = eventArgs.ChangeType,
                            InstanceId = InstanceId,
                            ProcessCount = processCount
                        };
                        _serializer.Serialize(statusWriter, status);
                        statusWriter.WriteLine();

                        // invoke the job function
                        var input = new TriggeredFunctionData
                        {
                            TriggerValue = eventArgs
                        };
                        result = await _executor.TryExecuteAsync(input, cancellationToken);

                        // write a status entry indicating the state of processing
                        status.State = result.Succeeded ? ProcessingState.Processed : ProcessingState.Failed;
                        status.Timestamp = DateTime.Now;
                        _serializer.Serialize(statusWriter, status);
                        statusWriter.WriteLine();

                        if (result.Succeeded)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the next retry interval to use.
        /// </summary>
        /// <param name="result">The <see cref="FunctionResult"/> for the last failure.</param>
        /// <param name="count">The current execution count (the number of times the function has
        /// been invoked).</param>
        /// <returns>A <see cref="TimeSpan"/> representing the delay interval that should be used.</returns>
        protected virtual TimeSpan GetRetryInterval(FunctionResult result, int count)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return TimeSpan.FromSeconds(3);
        }

        /// <summary>
        /// Perform any required cleanup. This includes deleting processed files
        /// (when <see cref="FileTriggerAttribute.AutoDelete"/> is True).
        /// </summary>
        public virtual void Cleanup()
        {
            if (_attribute.AutoDelete)
            {
                CleanupProcessedFiles();
            }
        }

        /// <summary>
        /// Determines whether the specified file should be processed.
        /// </summary>
        /// <param name="filePath">The candidate file for processing.</param>
        /// <returns>True if the file should be processed, false otherwise.</returns>
        public virtual bool ShouldProcessFile(string filePath)
        {
            if (IsStatusFile(filePath))
            {
                return false;
            }

            string statusFilePath = GetStatusFile(filePath);
            if (!File.Exists(statusFilePath))
            {
                return true;
            }

            StatusFileEntry statusEntry;
            try
            {
                GetLastStatus(statusFilePath, out statusEntry);
            }
            catch (IOException)
            {
                // if we get an exception reading the status file, it's
                // likely because someone started processing and has it locked
                return false;
            }

            return statusEntry is null || (statusEntry.State != ProcessingState.Processed &&
                statusEntry.ProcessCount < MaxProcessCount);
        }

        /// <summary>
        /// Clean up any files that have been fully processed
        /// </summary>
        public virtual void CleanupProcessedFiles()
        {
            int filesDeleted = 0;
            string[] statusFiles = Directory.GetFiles(_queueFolder, GetStatusFile("*"));
            foreach (string statusFilePath in statusFiles)
            {
                try
                {
                    // verify that the file has been fully processed
                    // if we're unable to get the last status or the file
                    // is not Processed, skip it
                    if (!GetLastStatus(statusFilePath, out var statusEntry) ||
                        statusEntry.State != ProcessingState.Processed)
                    {
                        continue;
                    }

                    // get all files starting with that file name. For example, for
                    // status file input.dat.status, this might return input.dat and
                    // input.dat.meta (if the file has other companion files)
                    string targetFileName = Path.GetFileNameWithoutExtension(statusFilePath);
                    string[] files = Directory.GetFiles(_queueFolder, targetFileName + "*");

                    // first delete the non status file(s)
                    foreach (string filePath in files)
                    {
                        if (IsStatusFile(filePath))
                        {
                            continue;
                        }

                        if (TryDelete(filePath))
                        {
                            filesDeleted++;
                        }
                    }

                    // then delete the status file
                    if (TryDelete(statusFilePath))
                    {
                        filesDeleted++;
                    }
                }
                catch
                {
                    // ignore any delete failures
                }
            }

            if (filesDeleted > 0)
            {
                _logger.LogDebug($"File Cleanup ({_queueFolder}): {filesDeleted} files deleted");
            }
        }

        /// <summary>
        /// Attempts to acquire a lock on the status file for the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file to lock.</param>
        /// <param name="changeType">The type of change detected on the file.</param>
        /// <param name="statusEntry">The status entry of the file.</param>
        /// <returns>A StreamWriter if the lock is acquired, null otherwise.</returns>
        internal StreamWriter AcquireStatusFileLock(string filePath, WatcherChangeTypes changeType, out StatusFileEntry statusEntry)
        {
            Stream stream = null;
            statusEntry = null;
            try
            {
                // Attempt to create (or update) the companion status file and lock it. The status
                // file is the mechanism for handling multi-instance concurrency.
                string statusFilePath = GetStatusFile(filePath);
                stream = File.Open(statusFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                // Once we've established the lock, we need to check to ensure that another instance
                // hasn't already processed the file in the time between our getting the event and
                // acquiring the lock.
                statusEntry = GetLastStatus(stream);
                if (statusEntry is not null && statusEntry.State == ProcessingState.Processed)
                {
                    // For file Create, we have no additional checks to perform. However for
                    // file Change, we need to also check the LastWrite value for the entry
                    // since there can be multiple Processed entries in the file over time.
                    if (changeType == WatcherChangeTypes.Created)
                    {
                        return null;
                    }
                    else if (changeType == WatcherChangeTypes.Changed &&
                        File.GetLastWriteTimeUtc(filePath) == statusEntry.LastWrite)
                    {
                        return null;
                    }
                }

                stream.Seek(0, SeekOrigin.End);
                var streamReader = new StreamWriter(stream)
                {
                    AutoFlush = true
                };
                stream = null;

                return streamReader;
            }
            catch
            {
                return null;
            }
            finally
            {
                stream?.Dispose();
            }
        }

        /// <summary>
        /// Gets the last status entry from the specified status file path.
        /// </summary>
        /// <param name="statusFilePath">The path of the status file.</param>
        /// <param name="statusEntry">The last status entry of the file.</param>
        /// <returns>True if the status entry is retrieved, false otherwise.</returns>
        internal bool GetLastStatus(string statusFilePath, out StatusFileEntry statusEntry)
        {
            statusEntry = null;

            if (!File.Exists(statusFilePath))
            {
                return false;
            }

            using (Stream stream = File.OpenRead(statusFilePath))
            {
                statusEntry = GetLastStatus(stream);
            }

            return statusEntry is not null;
        }

        /// <summary>
        /// Gets the last status entry from the specified status file stream.
        /// </summary>
        /// <param name="statusFileStream">The stream of the status file.</param>
        /// <returns>The last status entry of the file.</returns>
        internal StatusFileEntry GetLastStatus(Stream statusFileStream)
        {
            StatusFileEntry statusEntry = null;

            using (var reader = new StreamReader(statusFileStream, Encoding.UTF8, false, 1024, true))
            {
                string text = reader.ReadToEnd();
                string[] fileLines = text.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
                string lastLine = fileLines.LastOrDefault();
                if (!string.IsNullOrEmpty(lastLine))
                {
                    using var stringReader = new StringReader(lastLine);
                    statusEntry = (StatusFileEntry)_serializer.Deserialize(stringReader, typeof(StatusFileEntry));
                }
            }

            statusFileStream.Seek(0, SeekOrigin.End);

            return statusEntry;
        }

        /// <summary>
        /// Gets the status file path for the specified file.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>The status file path.</returns>
        internal string GetStatusFile(string file) => $"{file}.{StatusFileExtension}";

        /// <summary>
        /// Determines whether the specified file is a status file.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>True if the file is a status file, false otherwise.</returns>
        internal bool IsStatusFile(string file)
        {
            return string.Compare(Path.GetExtension(file).Trim('.'), StatusFileExtension, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Attempts to delete the specified file.
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
        /// <returns>True if the file is deleted, false otherwise.</returns>
        private static bool TryDelete(string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
