namespace CloudNimble.SimpleMessageBus.IndexedDb.Core
{

    /// <summary>
    /// Specifies the options required to leverage a browser's IndexedDB instance as the SimpleMessageBus backing queue.
    /// </summary>
    /// <remarks>
    /// These options configure the IndexedDB database and object store names used for message queuing
    /// in Blazor WebAssembly applications. The default values follow SimpleMessageBus conventions
    /// and can be customized for specific application requirements.
    /// </remarks>
    public class IndexedDbOptions
    {

        #region Properties

        /// <summary>
        /// The IndexedDb table where successfully-processed queue items will be moved to upon completion. Defaults to <see cref="IndexedDbConstants.Completed" />.
        /// </summary>
        public string CompletedQueueName { get; set; } = IndexedDbConstants.Completed;

        /// <summary>
        /// The name of the Database inside IndexedDb where the queue tables will be stored. Defaults to 'SimpleMessageBus'.
        /// </summary>
        public string DatabaseName { get; set; } = "SimpleMessageBus";

        /// <summary>
        /// The IndexedDb table where failed items will be stored while they are waiting to be analyzed and reprocessed. Defaults to <see cref="IndexedDbConstants.Error" />.
        /// </summary>
        public string ErrorQueueName { get; set; } = IndexedDbConstants.Error;

        /// <summary>
        /// The IndexedDb table where items will be stored while they are waiting to be processed. Defaults to <see cref="IndexedDbConstants.Queue" />.
        /// </summary>
        public string QueueName { get; set; } = IndexedDbConstants.Queue;

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor, which sets the default values equal to the values specified in <see cref="IndexedDbConstants"/>.
        /// </summary>
        public IndexedDbOptions()
        {
        }

        #endregion

    }

}