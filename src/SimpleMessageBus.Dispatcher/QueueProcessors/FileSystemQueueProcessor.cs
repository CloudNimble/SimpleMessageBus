using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// 
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
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dispatcher"></param>
        /// <param name="serviceProvider"></param>
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

        // Drop a file in the "convert" directory, and this function will reverse it
        // the contents and write the file to the "converted" directory.
        public void ProcessQueue(
            [FileTrigger(@"queue\{name}", "*.json", autoDelete: true)] string messageEnvelopeJson)
            //[File(@"%completed%\{name}", FileAccess.Write)] out string converted,
            //[File(@"%error%\{name}", FileAccess.Write)] out string error)
        {
            try
            {
                using (var lifetimeScope = _serviceProvider.CreateScope())
                {
                    var message = JsonConvert.DeserializeObject<MessageEnvelope>(messageEnvelopeJson);
                    //message.AttemptsCount = dequeueCount;
                    //message.ProcessLog = log;
                    _dispatcher.Dispatch(message).GetAwaiter().GetResult();
                    File.WriteAllText(Path.Combine(_options.RootFolder, _options.CompletedFolderPath, $"{message.Id}.json"), messageEnvelopeJson);
                }
                //converted = messageEnvelopeJson;
                //error = null;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(_options.RootFolder, _options.ErrorFolderPath), messageEnvelopeJson);
                //converted = null;
                //error = messageEnvelopeJson;
            }
        }


        #endregion

    }

}
