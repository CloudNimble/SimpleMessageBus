using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// 
    /// </summary>
    public class AzureStorageQueueProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="serviceProvider"></param>
        public AzureStorageQueueProcessor(IMessageDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "Please call \".UseOrderedMessageDispatcher()\" or \".UseParallelMessageDispatcher()\" in your Dependency Injection service registration.");
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(dispatcher), "The DependencyInjection IServiceProvider could not be found. Please ensure you've properly registered DI.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueMessage"></param>
        /// <param name="logger"></param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        [return: Queue(AzureStorageQueueConstants.CompletedQueueAttribute)]
        public async Task<CloudQueueMessage> ProcessQueue([QueueTrigger(AzureStorageQueueConstants.QueueTriggerAttribute)] CloudQueueMessage queueMessage, ILogger logger)
        {
            using var lifetimeScope = _serviceProvider.CreateScope();
            var message = JsonConvert.DeserializeObject<MessageEnvelope>(queueMessage.AsString);
            message.AttemptsCount = queueMessage.DequeueCount;
            message.ProcessLog = logger;
            message.ServiceScope = lifetimeScope;
            await _dispatcher.Dispatch(message).ConfigureAwait(false);
            //RWM: https://stackoverflow.com/questions/62333063/webjob-queuetrigger-does-not-delete-message-from-the-queue
            return new CloudQueueMessage(queueMessage.AsString);
        }

        #endregion

        #region Private Methods



        #endregion

    }

}