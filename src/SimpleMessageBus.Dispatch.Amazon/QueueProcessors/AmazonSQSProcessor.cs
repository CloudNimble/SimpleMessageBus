using Amazon.SQS.Model;
using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch.Amazon;
using CloudNimble.WebJobs.Extensions.Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Processes messages from Amazon SQS queues for SimpleMessageBus.
    /// </summary>
    public class AmazonSQSProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AmazonSQSProcessor"/>.
        /// </summary>
        /// <param name="dispatcher">The <see cref="IMessageDispatcher"/> to use for processing messages.</param>
        /// <param name="serviceScopeFactory">The <see cref="IServiceScopeFactory"/> to use for creating service scopes.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <see langword="null" /> or
        /// <paramref name="serviceScopeFactory"/> is <see langword="null" />.
        /// </exception>
        public AmazonSQSProcessor(IMessageDispatcher dispatcher, IServiceScopeFactory serviceScopeFactory)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "Please call \".UseOrderedMessageDispatcher()\" or \".UseParallelMessageDispatcher()\" in your Dependency Injection service registration.");
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory), "The DependencyInjection IServiceProvider could not be found. Please ensure you've properly registered DI.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes a message from the SQS queue.
        /// </summary>
        /// <param name="sqsMessage">The SQS message to process.</param>
        /// <param name="logger">The logger instance.</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        [return: SQS(AmazonSQSConstants.CompletedQueueAttribute)]
        public async Task<Message> ProcessQueue([SQSTrigger(AmazonSQSConstants.QueueTriggerAttribute)] Message sqsMessage, ILogger logger)
        {
            using var lifetimeScope = _serviceScopeFactory.CreateScope();
            var message = JsonSerializer.Deserialize<MessageEnvelope>(sqsMessage.Body);
            message.AttemptsCount = sqsMessage.Attributes.TryGetValue("ApproximateReceiveCount", out var count) ? int.Parse(count) : 1;
            message.ProcessLog = logger;
            message.ServiceScope = lifetimeScope;
            await _dispatcher.Dispatch(message).ConfigureAwait(false);
            return sqsMessage;
        }

        #endregion

    }

}