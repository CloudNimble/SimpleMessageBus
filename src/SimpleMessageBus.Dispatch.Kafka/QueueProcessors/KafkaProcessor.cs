using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch.Kafka
{

    /// <summary>
    /// Processes messages from Apache Kafka and dispatches them to registered message handlers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This processor integrates with Azure WebJobs to automatically trigger message processing
    /// when messages arrive in a Kafka topic. It handles message deserialization, lifecycle
    /// management, and provides proper logging and dependency injection scope for each message.
    /// </para>
    /// <para>
    /// Unlike Azure Storage Queues, Kafka does not require explicit message deletion. The WebJobs
    /// Kafka extension automatically commits offsets after successful processing. If processing
    /// fails, the message will be reprocessed based on the consumer group's offset configuration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// Host.CreateDefaultBuilder()
    ///     .ConfigureServices(services =>
    ///     {
    ///         services.AddSingleton&lt;IMessageHandler, MyMessageHandler&gt;();
    ///     })
    ///     .UseKafkaProcessor(options =>
    ///     {
    ///         options.BrokerList = "localhost:9092";
    ///         options.ConsumerGroup = "my-consumer";
    ///     })
    ///     .UseOrderedMessageDispatcher()
    ///     .Build()
    ///     .Run();
    /// </code>
    /// </example>
    public class KafkaProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="KafkaProcessor"/>.
        /// </summary>
        /// <param name="dispatcher">The message dispatcher to route messages to handlers.</param>
        /// <param name="serviceScopeFactory">Factory for creating DI scopes per message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dispatcher"/> or <paramref name="serviceScopeFactory"/> is null.
        /// </exception>
        public KafkaProcessor(IMessageDispatcher dispatcher, IServiceScopeFactory serviceScopeFactory)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher),
                "Please call \".UseOrderedMessageDispatcher()\" or \".UseParallelMessageDispatcher()\" in your Dependency Injection service registration.");
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory),
                "The DependencyInjection IServiceProvider could not be found.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes a message from the Kafka topic and dispatches it to registered handlers.
        /// </summary>
        /// <param name="kafkaEvent">The Kafka event containing the message payload.</param>
        /// <param name="logger">The logger instance for this processing operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// <para>
        /// This method is triggered automatically by the WebJobs Kafka extension when messages
        /// arrive. It deserializes the message envelope, sets up processing context, and
        /// dispatches to handlers.
        /// </para>
        /// <para>
        /// The offset is automatically committed after successful processing. If an exception
        /// is thrown, the offset is not committed and the message will be reprocessed.
        /// </para>
        /// </remarks>
        public async Task ProcessKafkaMessage(
            [KafkaTrigger(KafkaConstants.BrokerListAttribute, KafkaConstants.TopicTriggerAttribute,
                ConsumerGroup = KafkaConstants.ConsumerGroupAttribute)]
            KafkaEventData<string> kafkaEvent,
            ILogger logger)
        {
            using var lifetimeScope = _serviceScopeFactory.CreateScope();

            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(kafkaEvent.Value);

            // Kafka doesn't have a built-in dequeue count like Azure Queues
            envelope.AttemptsCount = 1;
            envelope.ProcessLog = logger;
            envelope.ServiceScope = lifetimeScope;

            // Log Kafka-specific metadata for debugging
            logger.LogDebug("Processing Kafka message: Partition={Partition}, Offset={Offset}, Key={Key}, Timestamp={Timestamp}",
                kafkaEvent.Partition, kafkaEvent.Offset, kafkaEvent.Key, kafkaEvent.Timestamp);

            await _dispatcher.Dispatch(envelope).ConfigureAwait(false);

            // Note: Offset is automatically committed by the WebJobs extension
            // after this method completes successfully
        }

        #endregion

    }

}
