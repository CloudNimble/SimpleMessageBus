namespace CloudNimble.SimpleMessageBus.Dispatch.Amazon
{

    /// <summary>
    /// A set of constants for SimpleMessageBus instances backed by Amazon SQS.
    /// </summary>
    public static class AmazonSQSConstants
    {

        #region Properties

        /// <summary>
        /// The WebJobs attribute name for the name of the completed messages queue.
        /// </summary>
        public const string CompletedQueueAttribute = "%CompletedQueueName%";

        /// <summary>
        /// The WebJobs attribute name for the name of the poison messages queue.
        /// </summary>
        public const string PoisonQueueAttribute = "%PoisonQueueName%";

        /// <summary>
        /// The WebJobs attribute name for the name of the queue triggers.
        /// </summary>
        public const string QueueTriggerAttribute = "%QueueName%";

        #endregion

    }

}