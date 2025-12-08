namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Specifies the options required to leverage Apache Kafka as the SimpleMessageBus backing queue.
    /// </summary>
    public class KafkaOptions
    {

        #region Properties

        /// <summary>
        /// The SASL authentication mechanism when using SASL protocols.
        /// </summary>
        public KafkaAuthenticationMode AuthenticationMode { get; set; } = KafkaAuthenticationMode.NotSet;

        /// <summary>
        /// The Kafka broker list (e.g., "localhost:9092" or "broker1:9092,broker2:9092").
        /// </summary>
        public string BrokerList { get; set; }

        /// <summary>
        /// The consumer group ID for message consumption.
        /// </summary>
        public string ConsumerGroup { get; set; }

        /// <summary>
        /// Maximum number of messages to process in a batch. Default is 64.
        /// </summary>
        public int MaxBatchSize { get; set; } = 64;

        /// <summary>
        /// SASL password for authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The security protocol for broker communication.
        /// </summary>
        public KafkaBrokerProtocol Protocol { get; set; } = KafkaBrokerProtocol.Plaintext;

        /// <summary>
        /// Path to CA certificate file for SSL/TLS verification.
        /// </summary>
        public string SslCaLocation { get; set; }

        /// <summary>
        /// The name of the Kafka topic for messages.
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// SASL username for authentication.
        /// </summary>
        public string Username { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance with default values from <see cref="KafkaConstants"/>.
        /// </summary>
        public KafkaOptions()
        {
            TopicName = KafkaConstants.Topic;
            ConsumerGroup = KafkaConstants.DefaultConsumerGroup;
        }

        #endregion

    }

}
