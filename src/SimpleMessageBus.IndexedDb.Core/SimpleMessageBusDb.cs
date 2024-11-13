using CloudNimble.BlazorEssentials.IndexedDb;
using CloudNimble.SimpleMessageBus.IndexedDb.Core;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace SimpleMessageBus.IndexedDb.Core
{

    /// <summary>
    /// 
    /// </summary>
    public class SimpleMessageBusDb : IndexedDbDatabase
    {

        private IndexedDbOptions IndexedDbOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IndexedDbObjectStore Completed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IndexedDbObjectStore Failed { get; set; }

        /// <summary>
        /// 
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
