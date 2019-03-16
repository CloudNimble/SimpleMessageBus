using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;

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
            if (string.IsNullOrWhiteSpace(options.Value.QueueFolderPath))
            {
                throw new ArgumentNullException(nameof(options.Value.QueueFolderPath), "Please specify the folder that will store queue items.");
            }
            if (string.IsNullOrWhiteSpace(options.Value.CompletedFolderPath))
            {
                throw new ArgumentNullException(nameof(options.Value.CompletedFolderPath), "Please specify the folder that will store items that completed successfully.");
            }
            if (string.IsNullOrWhiteSpace(options.Value.ErrorFolderPath))
            {
                throw new ArgumentNullException(nameof(options.Value.ErrorFolderPath), "Please specify the folder that will store items that could not be processed.");
            }

            _options = options.Value;
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageEnvelopeJson"></param>
        /// <param name="logger"></param>
        public void ProcessQueue(
            [FileTrigger(@"queue\{name}", "*.json", autoDelete: true)] string messageEnvelopeJson, ILogger logger)
            //[File(@"%completed%\{name}", FileAccess.Write)] out string converted,
            //[File(@"%error%\{name}", FileAccess.Write)] out string error)
        {
            MessageEnvelope messageEnvelope = null;
            try
            {
                using (var lifetimeScope = _serviceProvider.CreateScope())
                {
                    messageEnvelope = JsonConvert.DeserializeObject<MessageEnvelope>(messageEnvelopeJson);
                    //message.AttemptsCount = dequeueCount;
                    //message.ProcessLog = log;
                    _dispatcher.Dispatch(messageEnvelope).GetAwaiter().GetResult();
                    File.WriteAllText(Path.Combine(_options.RootFolder, _options.CompletedFolderPath, $"{messageEnvelope.Id}.json"), messageEnvelopeJson);
                }
                //converted = messageEnvelopeJson;
                //error = null;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogCritical(ex, "An error occurred dispatching the MessageEnvelope with ID {0}", messageEnvelope?.Id);
                }
                File.WriteAllText(Path.Combine(_options.RootFolder, _options.ErrorFolderPath), messageEnvelopeJson);
            }
        }


        #endregion

    }

}
