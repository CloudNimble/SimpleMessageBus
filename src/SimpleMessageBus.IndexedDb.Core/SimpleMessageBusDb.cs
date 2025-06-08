using CloudNimble.BlazorEssentials.IndexedDb;
using CloudNimble.SimpleMessageBus.IndexedDb.Core;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace SimpleMessageBus.IndexedDb.Core
{

    /// <summary>
    /// Represents the IndexedDB database structure for SimpleMessageBus in Blazor WebAssembly applications.
    /// </summary>
    /// <remarks>
    /// This class defines the IndexedDB schema used for client-side message queuing in Blazor WebAssembly.
    /// It provides three object stores for managing message lifecycle: Queue (pending), Completed (successful),
    /// and Failed (error) processing states.
    /// </remarks>
    public class SimpleMessageBusDb : IndexedDbDatabase
    {

        private IndexedDbOptions IndexedDbOptions { get; set; }

        /// <summary>
        /// Gets or sets the object store for successfully processed messages.
        /// </summary>
        public IndexedDbObjectStore Completed { get; set; }

        /// <summary>
        /// Gets or sets the object store for messages that failed processing.
        /// </summary>
        public IndexedDbObjectStore Failed { get; set; }

        /// <summary>
        /// Gets or sets the object store for messages pending processing.
        /// </summary>
        public IndexedDbObjectStore Queue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="indexedDbOptions"></param>
        public SimpleMessageBusDb(IJSRuntime jsRuntime, IOptions<IndexedDbOptions> indexedDbOptions) : base(jsRuntime)
        {
            IndexedDbOptions = indexedDbOptions.Value;
            Name = IndexedDbOptions.DatabaseName;
            Completed = new IndexedDbObjectStore(this, IndexedDbOptions.CompletedQueueName);
            Failed = new IndexedDbObjectStore(this, IndexedDbOptions.ErrorQueueName);
            Queue = new IndexedDbObjectStore(this, IndexedDbOptions.QueueName);
        }

    }

}
