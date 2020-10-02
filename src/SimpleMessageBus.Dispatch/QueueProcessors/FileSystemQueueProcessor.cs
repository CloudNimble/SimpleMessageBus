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
    public class FileSystemQueueProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly FileSystemOptions _options;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor called by the Dependency Injection container.
        /// </summary>
        /// <param name="options">
        /// The injected <see cref="IOptions{FileSystemOptions}"/> specifying the options required to leverage the local file system as the SimpleMessageBus backing queue.
        /// </param>
        /// <param name="dispatcher"></param>
        /// <param name="serviceProvider">
        /// The Dependency Injection container's <see cref="IServiceProvider"/> instance, so that a "per-request" scope can be created that gives each <see cref="MessageEnvelope"/>
        /// its own set of isolated dependencies.
        /// </param>
        public FileSystemQueueProcessor(IOptions<FileSystemOptions> options, IMessageDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Please register a FileSystemOptions instance with your DI container.");
            }
            if (string.IsNullOrWhiteSpace(options.Value.RootFolder))
            {
                throw new ArgumentNullException(nameof(options.Value.RootFolder), "Please specify the root path that will contain the folders for processing the queue.");
            }

            _options = options.Value;
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;

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
        /// 
        /// </summary>
        /// <param name="messageEnvelopeJson"></param>
        /// <param name="fileTrigger"></param>
        /// <param name="logger"></param>
        public async Task ProcessQueue(
#pragma warning disable IDE0060 // Remove unused parameter
            [SimpleMessageBusFileTrigger(@"queue\{name}", "*.json", WatcherChangeTypes.Created | WatcherChangeTypes.Renamed, true)] string messageEnvelopeJson, FileSystemEventArgs fileTrigger, ILogger logger)
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
                using var lifetimeScope = _serviceProvider.CreateScope();
                messageEnvelope = JsonConvert.DeserializeObject<MessageEnvelope>(messageEnvelopeJson);
                //messageEnvelope.AttemptsCount = dequeueCount;
                messageEnvelope.ProcessLog = logger;
                await _dispatcher.Dispatch(messageEnvelope);
                File.WriteAllText(Path.Combine(_options.CompletedFolderPath, $"{messageEnvelope.Id}.json"), messageEnvelopeJson);
                //converted = messageEnvelopeJson;
                //error = null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                if (logger != null)
                {
                    logger.LogCritical(ex, "An error occurred dispatching the MessageEnvelope with ID {0}", messageEnvelope?.Id);
                }
                File.WriteAllText(Path.Combine(_options.ErrorFolderPath, $"{messageEnvelope.Id}.json"), messageEnvelopeJson);
            }
        }

        #endregion

    }

}
