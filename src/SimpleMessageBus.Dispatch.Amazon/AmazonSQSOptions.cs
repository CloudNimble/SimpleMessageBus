using CloudNimble.WebJobs.Extensions.Amazon.SQS;

namespace CloudNimble.SimpleMessageBus.Dispatch.Amazon
{

    /// <summary>
    /// Defines the configuration options available for SimpleMessageBus queues backed by Amazon SQS.
    /// </summary>
    public class AmazonSQSOptions : SQSOptions
    {

        #region Properties

        /// <summary>
        /// The name of the queue to process for completed messages.
        /// </summary>
        public string CompletedQueueName { get; set; }

        /// <summary>
        /// The name of the queue to process for poison messages.
        /// </summary>
        public string PoisonQueueName { get; set; }

        #endregion

    }

}