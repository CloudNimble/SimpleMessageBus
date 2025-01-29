using Azure.Storage.Queues.Models;
using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="serviceScopeFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AzureStorageQueueProcessor(IMessageDispatcher dispatcher, IServiceScopeFactory serviceScopeFactory)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "Please call \".UseOrderedMessageDispatcher()\" or \".UseParallelMessageDispatcher()\" in your Dependency Injection service registration.");
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(dispatcher), "The DependencyInjection IServiceProvider could not be found. Please ensure you've properly registered DI.");
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
        public async Task<QueueMessage> ProcessQueue([QueueTrigger(AzureStorageQueueConstants.QueueTriggerAttribute)] QueueMessage queueMessage, ILogger logger)
        {
            using var lifetimeScope = _serviceScopeFactory.CreateScope();
            var message = queueMessage.Body.ToObjectFromJson<MessageEnvelope>();
            message.AttemptsCount = queueMessage.DequeueCount;
            message.ProcessLog = logger;
            message.ServiceScope = lifetimeScope;
            await _dispatcher.Dispatch(message).ConfigureAwait(false);
            //RWM: https://stackoverflow.com/questions/62333063/webjob-queuetrigger-does-not-delete-message-from-the-queue
            //return new QueueMessage(queueMessage.AsString);
            return queueMessage;
        }

        #endregion

        #region Private Methods



        #endregion

    }

}