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
    /// Processes messages from Azure Storage Queues and dispatches them to registered message handlers.
    /// </summary>
    /// <remarks>
    /// This processor integrates with Azure WebJobs to automatically trigger message processing when
    /// messages arrive in Azure Storage Queues. It handles message deserialization, lifecycle management,
    /// and provides proper logging and dependency injection scope for each message.
    /// </remarks>
    public class AzureStorageQueueProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageQueueProcessor"/> class.
        /// </summary>
        /// <param name="dispatcher">The message dispatcher to route messages to handlers.</param>
        /// <param name="serviceScopeFactory">The service scope factory for creating DI scopes per message.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dispatcher"/> or <paramref name="serviceScopeFactory"/> is null.</exception>
        public AzureStorageQueueProcessor(IMessageDispatcher dispatcher, IServiceScopeFactory serviceScopeFactory)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "Please call \".UseOrderedMessageDispatcher()\" or \".UseParallelMessageDispatcher()\" in your Dependency Injection service registration.");
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(dispatcher), "The DependencyInjection IServiceProvider could not be found. Please ensure you've properly registered DI.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes a message from the Azure Storage Queue and dispatches it to registered handlers.
        /// </summary>
        /// <param name="queueMessage">The Azure Storage Queue message to process.</param>
        /// <param name="logger">The logger instance for this processing operation.</param>
        /// <returns>The processed queue message, which will be moved to the completed queue if configured.</returns>
        /// <remarks>
        /// This method is triggered automatically by the Azure WebJobs framework when messages arrive.
        /// It deserializes the message envelope, sets up processing context, and dispatches to handlers.
        /// If processing succeeds, the message is optionally moved to a completion queue.
        /// </remarks>
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