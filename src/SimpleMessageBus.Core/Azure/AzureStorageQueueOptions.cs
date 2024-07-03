namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Specifies the options required to leverage Azure Queue Storage as the SimpleMessageBus backing queue.
    /// </summary>
    public class AzureStorageQueueOptions
    {

        #region Properties

        /// <summary>
        /// A <see cref="string"/> representing the name of the Queue that successfully-executed messages will be stored in.
        /// </summary>
        /// <remarks>
        /// Messages will stay in that Queue for the lifetime specified by the Queue, and is useful for 
        /// diagnosing or re-running requests.
        /// </remarks>
        public string CompletedQueueName { get; set; }

        /// <summary>
        /// An <see cref="int"/> representing the number of Messages that can be processed simultaneously. The default is 16.
        /// </summary>
        public int ConcurrentJobs { get; set; } = 16;

        /// <summary>
        /// Sets the MessageEncoding for Queue messages. Defaults to AzureStorageQueueEncoding.None.
        /// </summary>
        /// <remarks>
        /// SimpleMessageBus defaulted to None in previous versions. This setting helps align the Azure Queues SDK QueueClient,
        /// which by default sets MessageEncoding = QueueMessageEncoding.None, and WebJobs SDK QueueTrigger, which by default
        /// sets MessageEncoding = QueueMessageEncoding.Base64. (I know, isn't that awesome?!?).
        /// </remarks>
        public AzureStorageQueueEncoding MessageEncoding { get; set; } = AzureStorageQueueEncoding.None;

        /// <summary>
        /// A <see cref="string"/> representing the name of the Queue in Azure Queue Storage.
        /// </summary>
        /// <remarks>
        /// See https://coderwall.com/p/g2xeua for more information about queue name requirements.
        /// </remarks>
        public string QueueName { get; set; }

        /// <summary>
        /// A <see cref="string"/> representing the ConnectionString for your Azure Storage account.
        /// </summary>
        public string StorageConnectionString { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor, which sets the default values equal to the values specified in <see cref="AzureStorageQueueConstants"/>.
        /// </summary>
        public AzureStorageQueueOptions()
        {
            QueueName = AzureStorageQueueConstants.Queue;
            CompletedQueueName = AzureStorageQueueConstants.CompletedQueue;
        }

        #endregion

    }

}