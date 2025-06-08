using CloudNimble.WebJobs.Extensions.Amazon.SQS;

namespace CloudNimble.SimpleMessageBus.Amazon.Core
{

    /// <summary>
    /// Defines the configuration options available for SimpleMessageBus queues backed by Amazon SQS.
    /// </summary>
    /// <remarks>
    /// This class extends the base SQS options to provide SimpleMessageBus-specific configuration for Amazon SQS queues.
    /// It defines the three-queue pattern used by SimpleMessageBus: a main queue for processing, a poison queue for
    /// failed messages, and an optional completed queue for successfully processed messages.
    /// 
    /// The SimpleMessageBus Amazon SQS provider follows AWS best practices for message processing and error handling,
    /// including dead letter queues, visibility timeouts, and message retention policies.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Configure Amazon SQS options in appsettings.json
    /// {
    ///   "SimpleMessageBus": {
    ///     "Amazon": {
    ///       "QueueName": "myapp-messages",
    ///       "PoisonQueueName": "myapp-messages-poison",
    ///       "CompletedQueueName": "myapp-messages-completed",
    ///       "Region": "us-west-2",
    ///       "VisibilityTimeoutInSeconds": 300,
    ///       "WaitTimeSeconds": 20
    ///     }
    ///   }
    /// }
    /// 
    /// // Or configure programmatically
    /// services.Configure&lt;AmazonSQSOptions&gt;(options =&gt;
    /// {
    ///     options.QueueName = "myapp-messages";
    ///     options.PoisonQueueName = "myapp-messages-poison";
    ///     options.CompletedQueueName = "myapp-messages-completed";
    ///     options.Region = "us-west-2";
    /// });
    /// </code>
    /// </example>
    public class AmazonSQSOptions : SQSOptions
    {

        #region Properties

        /// <summary>
        /// Gets or sets the name of the queue to process for completed messages.
        /// </summary>
        /// <value>
        /// The SQS queue name where successfully processed messages are optionally stored. 
        /// This queue provides an audit trail of completed processing and can be used for analytics or compliance.
        /// </value>
        /// <remarks>
        /// The completed queue is optional. If not specified, successfully processed messages are simply deleted
        /// from the main queue. When specified, messages are moved to this queue after successful processing,
        /// allowing for audit trails and processing analytics.
        /// 
        /// Queue names must follow AWS SQS naming conventions: 1-80 characters, alphanumeric plus hyphens and underscores.
        /// </remarks>
        /// <example>
        /// <code>
        /// options.CompletedQueueName = "myapp-messages-completed";
        /// // This queue will contain messages that were successfully processed
        /// </code>
        /// </example>
        public string CompletedQueueName { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue to process for the main messages.
        /// </summary>
        /// <value>
        /// The primary SQS queue name where messages are published and from which they are consumed for processing.
        /// This is the main message processing queue in the SimpleMessageBus workflow.
        /// </value>
        /// <remarks>
        /// This is the primary queue where all messages are initially published and from which message handlers
        /// consume messages for processing. This queue should be configured with appropriate visibility timeouts,
        /// message retention periods, and dead letter queue policies.
        /// 
        /// Queue names must follow AWS SQS naming conventions: 1-80 characters, alphanumeric plus hyphens and underscores.
        /// Consider using environment-specific prefixes for queue names to avoid conflicts between environments.
        /// </remarks>
        /// <example>
        /// <code>
        /// options.QueueName = "myapp-messages";
        /// // or environment-specific naming
        /// options.QueueName = $"myapp-{environment}-messages";
        /// </code>
        /// </example>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue to process for poison messages.
        /// </summary>
        /// <value>
        /// The SQS queue name where messages that fail processing multiple times are moved.
        /// This queue serves as a dead letter queue for failed message processing.
        /// </value>
        /// <remarks>
        /// The poison queue (dead letter queue) contains messages that have exceeded the maximum retry attempts
        /// and could not be processed successfully. These messages require manual intervention or specialized
        /// error handling workflows. The poison queue should have longer message retention periods to allow
        /// for investigation and recovery.
        /// 
        /// It's recommended to monitor this queue for volume and patterns of failures, as high poison message
        /// volumes may indicate systemic issues in message processing or external dependencies.
        /// 
        /// Queue names must follow AWS SQS naming conventions: 1-80 characters, alphanumeric plus hyphens and underscores.
        /// </remarks>
        /// <example>
        /// <code>
        /// options.PoisonQueueName = "myapp-messages-poison";
        /// // Configure with longer retention for investigation
        /// // Queue should be monitored for failure patterns
        /// </code>
        /// </example>
        public string PoisonQueueName { get; set; }

        #endregion

    }

}