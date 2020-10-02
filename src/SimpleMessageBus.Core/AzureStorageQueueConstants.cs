namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// A set of helpers to convert file system-related magic strings to compiled references.
    /// </summary>
    public static class AzureStorageQueueConstants
    {

        /// <summary>
        /// 
        /// </summary>
        public const string Queue = "queue";

        /// <summary>
        /// 
        /// </summary>
        public const string QueueTriggerAttribute = "%queue%";

        /// <summary>
        /// 
        /// </summary>
        public const string CompletedQueueAttribute = "%completedqueue%";

        /// <summary>
        /// 
        /// </summary>
        public const string CompletedQueue = "queue-completed";

        ///
        public const string LocalConnectionString = "UseDevelopmentStorage=true";

    }

}