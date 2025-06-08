using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch.Triggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Processes queue items stored in the local file system and dispatches them to all <see cref="IMessageHandler">IMessageHandlers</see> registered with the DI container.
    /// </summary>
    /// <remarks>
    /// This processor monitors a configured file system directory for new message files and automatically
    /// processes them when they appear. It supports three-folder operation: queue (incoming), completed
    /// (successfully processed), and error (failed processing). The processor integrates with Azure WebJobs
    /// for file system monitoring and automatic triggering.
    /// </remarks>
    public class FileSystemQueueProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly FileSystemOptions _options;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor called by the Dependency Injection container.
        /// </summary>
        /// <param name="options">
        /// The injected <see cref="IOptions{FileSystemOptions}"/> specifying the options required to leverage the local file system as the SimpleMessageBus backing queue.
        /// </param>
        /// <param name="dispatcher">The message dispatcher to route messages to handlers.</param>
        /// <param name="serviceScopeFactory">
        /// The Dependency Injection container's <see cref="IServiceProvider"/> instance, so that a "per-request" scope can be created that gives each <see cref="MessageEnvelope"/>
        /// its own set of isolated dependencies.
        /// </param>
        public FileSystemQueueProcessor(IOptions<FileSystemOptions> options, IMessageDispatcher dispatcher, IServiceScopeFactory serviceScopeFactory)
        {
            if (options?.Value is null)
            {
                throw new ArgumentNullException(nameof(options), "Please call HostBuilder.AddFileSystemQueueProcessor() in your application startup, " +
                    "and check that your appsettings.json file is set to be copied to the Output directory.");
            }
            if (string.IsNullOrWhiteSpace(options.Value.RootFolder))
            {
                throw new ArgumentNullException(nameof(options.Value.RootFolder), "Please specify the root path that will contain the folders for processing the queue.");
            }

            _options = options.Value;
            _dispatcher = dispatcher;
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            if (!Directory.Exists(_options.QueueFolderPath))
            {
                Directory.CreateDirectory(_options.QueueFolderPath);
            }

            if (!Directory.Exists(_options.CompletedFolderPath))
            {
                Directory.CreateDirectory(_options.CompletedFolderPath);
            }

            if (!Directory.Exists(_options.ErrorFolderPath))
            {
                Directory.CreateDirectory(_options.ErrorFolderPath);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes a message file when it appears in the queue directory.
        /// </summary>
        /// <param name="messageEnvelopeJson">The JSON content of the message envelope file.</param>
        /// <param name="fileTrigger">The file system event that triggered this processing.</param>
        /// <param name="logger">The logger instance for this processing operation.</param>
        /// <remarks>
        /// This method is triggered automatically by the Azure WebJobs framework when files are created
        /// or renamed in the queue directory. It deserializes the message and dispatches it to handlers.
        /// </remarks>
        public async Task ProcessQueue(
#pragma warning disable IDE0060 // Remove unused parameter
            [SimpleMessageBusFileTrigger(@"%queue%", "*.json", WatcherChangeTypes.Created | WatcherChangeTypes.Renamed, true)] string messageEnvelopeJson, FileSystemEventArgs fileTrigger, ILogger logger)
#pragma warning restore IDE0060 // Remove unused parameter
                               //[File(@"%completed%\{name}", FileAccess.Write)] out string converted,
                               //[File(@"%error%\{name}", FileAccess.Write)] out string error)
        {
            if (string.IsNullOrEmpty(messageEnvelopeJson))
            {
                throw new ArgumentException("message", nameof(messageEnvelopeJson));
            }

            if (fileTrigger is null)
            {
                throw new ArgumentNullException(nameof(fileTrigger));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            MessageEnvelope messageEnvelope = null;
            try
            {
                using var lifetimeScope = _serviceScopeFactory.CreateScope();
                messageEnvelope = JsonConvert.DeserializeObject<MessageEnvelope>(messageEnvelopeJson);
                //messageEnvelope.AttemptsCount = dequeueCount;
                messageEnvelope.ProcessLog = logger;
                messageEnvelope.ServiceScope = lifetimeScope;
                await _dispatcher.Dispatch(messageEnvelope);
                File.WriteAllText(Path.Combine(_options.CompletedFolderPath, $"{messageEnvelope.Id}.json"), messageEnvelopeJson);
                //converted = messageEnvelopeJson;
                //error = null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                logger?.LogCritical(ex, "An error occurred dispatching the MessageEnvelope with ID {0}", messageEnvelope?.Id);
                File.WriteAllText(Path.Combine(_options.ErrorFolderPath, $"{messageEnvelope.Id}.json"), messageEnvelopeJson);
            }
        }

        #endregion

    }

}
