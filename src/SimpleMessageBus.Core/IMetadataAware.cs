using System.Collections.Concurrent;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines a message that supports metadata for passing data between handlers.
    /// </summary>
    public interface IMetadataAware
    {

        /// <summary>
        /// Thread-safe metadata storage for passing data between handlers in the processing pipeline.
        /// </summary>
        ConcurrentDictionary<string, object> Metadata { get; set; }

    }

}