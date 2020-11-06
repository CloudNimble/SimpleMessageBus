namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Specifies the options required to leverage Azure Queue Storage as the SimpleMessageBus backing queue.
    /// </summary>
    public class AzureStorageQueueOptions
    {

        #region Properties

        /// <summary>
        /// A string representing the name of the Queue in Azure Queue Storage.
        /// </summary>
        /// <remarks>
        /// See https://coderwall.com/p/g2xeua for more information about queue name requirements.
        /// </remarks>
        public string QueueName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CompletedQueueName { get; set; }

        /// <summary>
        /// A string representing the Connection String for your Azure Storage account.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ConcurrentJobs { get; set; } = 16;

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