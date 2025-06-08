using CloudNimble.SimpleMessageBus.Core;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Concurrent
{

    /// <summary>
    /// Extension methods for the <see cref="ConcurrentDictionary{TKey, TValue}"/> class.
    /// </summary>
    public static class SimpleMessageBus_ConcurrentDictionaryExtensions
    {

        #region Public Methods

        /// <summary>
        /// Filters the metadata dictionary to exclude keys ending with "-Status" and combines it with the payload dictionary.
        /// </summary>
        /// <param name="metadata">The metadata dictionary to filter and combine.</param>
        /// <param name="payload">The payload dictionary to combine with the filtered metadata.</param>
        /// <returns>A new dictionary containing the combined entries from the payload and filtered metadata.</returns>
        /// <remarks>
        /// This method helps filter out handler execution status from the metadata when constructing 
        /// new messages or passing metadata between events in a processing chain.
        /// </remarks>
        public static Dictionary<string, object> FilterAndCombine(this ConcurrentDictionary<string, object> metadata, Dictionary<string, object> payload)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(payload);

            // Create a copy to avoid modifying the original payload
            var result = payload.ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in metadata.Where(kvp => !kvp.Key.EndsWith("-Status") && !kvp.Key.EndsWith("-Timestamp")))
            {
                result[item.Key] = item.Value; // Use indexer to update value if key exists
            }

            return result;
        }

        /// <summary>
        /// Filters the metadata <see cref="ConcurrentDictionary{TKey, TValue}" /> to exclude keys ending with "-Status" or "-Timestamp".
        /// </summary>
        /// <param name="metadata">The metadata dictionary to filter.</param>
        /// <returns>A new concurrent dictionary containing only the non-status entries.</returns>
        /// <remarks>
        /// This method helps filter out handler execution metadata when copying metadata between events,
        /// ensuring that the execution status of one event doesn't affect another.
        /// </remarks>
        public static ConcurrentDictionary<string, object> Filter(this ConcurrentDictionary<string, object> metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return new ConcurrentDictionary<string, object>(
                metadata.Where(kvp => !kvp.Key.EndsWith("-Status") && !kvp.Key.EndsWith("-Timestamp"))
            );
        }

        #endregion

    }

}