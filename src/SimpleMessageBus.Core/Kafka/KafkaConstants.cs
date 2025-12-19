namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Constants for Kafka topic and consumer group configuration placeholders.
    /// </summary>
    public static class KafkaConstants
    {

        #region Properties

        /// <summary>
        /// WebJobs attribute placeholder for broker list resolution.
        /// </summary>
        public const string BrokerListAttribute = "%brokerlist%";

        /// <summary>
        /// WebJobs attribute placeholder for consumer group resolution.
        /// </summary>
        public const string ConsumerGroupAttribute = "%consumergroup%";

        /// <summary>
        /// Default consumer group name.
        /// </summary>
        public const string DefaultConsumerGroup = "simplemessagebus-consumer";

        /// <summary>
        /// Default topic name for SimpleMessageBus messages.
        /// </summary>
        public const string Topic = "simplemessagebus";

        /// <summary>
        /// WebJobs attribute placeholder for topic name resolution.
        /// </summary>
        public const string TopicTriggerAttribute = "%topic%";

        #endregion

    }

}
