using System.Collections.Concurrent;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines a message that supports metadata for passing data between handlers in the processing pipeline.
    /// </summary>
    /// <remarks>
    /// Metadata provides a way to attach additional context and data to messages without modifying the core message structure.
    /// This is particularly useful for cross-cutting concerns like tracking, auditing, user context, and handler-to-handler
    /// communication. The metadata survives serialization and can be accessed by any handler in the processing chain.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class OrderCreatedMessage : IMessage, IMetadataAware
    /// {
    ///     public Guid Id { get; set; } = Guid.NewGuid();
    ///     public ConcurrentDictionary&lt;string, object&gt; Metadata { get; set; } = new();
    ///     public string OrderNumber { get; set; }
    ///     public decimal Amount { get; set; }
    /// }
    /// 
    /// // Usage in handler
    /// public async Task OnNextAsync(MessageEnvelope envelope)
    /// {
    ///     if (envelope.Message is IMetadataAware metadataMessage)
    ///     {
    ///         // Read metadata set by previous handlers
    ///         var userId = metadataMessage.Metadata.GetValueOrDefault("UserId");
    ///         var requestId = metadataMessage.Metadata.GetValueOrDefault("RequestId");
    ///         
    ///         // Add new metadata for downstream handlers
    ///         metadataMessage.Metadata["ProcessedBy"] = "OrderHandler";
    ///         metadataMessage.Metadata["ProcessedAt"] = DateTime.UtcNow;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IMetadataAware
    {

        /// <summary>
        /// Gets or sets the thread-safe metadata storage for passing data between handlers in the processing pipeline.
        /// </summary>
        /// <value>
        /// A <see cref="ConcurrentDictionary{TKey, TValue}"/> where keys are string identifiers and values are objects
        /// containing the metadata. The dictionary must be thread-safe as it may be accessed concurrently.
        /// </value>
        /// <remarks>
        /// This metadata dictionary is preserved during message serialization/deserialization and provides a mechanism
        /// for handlers to communicate with each other without coupling their interfaces. Common use cases include
        /// tracking user context, request IDs, processing timestamps, and handler-specific state.
        /// 
        /// When using metadata, prefer well-known key names and document their usage to avoid conflicts between handlers.
        /// Values should be serializable types to ensure compatibility across different storage providers.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Setting metadata in a publisher
        /// var message = new OrderCreatedMessage { OrderNumber = "ORD-001" };
        /// message.Metadata["UserId"] = currentUser.Id;
        /// message.Metadata["RequestId"] = Guid.NewGuid();
        /// message.Metadata["Source"] = "WebAPI";
        /// 
        /// // Reading metadata in a handler
        /// var userId = message.Metadata.GetValueOrDefault("UserId")?.ToString();
        /// var isFromAPI = message.Metadata.ContainsKey("Source") &amp;&amp; 
        ///                 message.Metadata["Source"].ToString() == "WebAPI";
        /// </code>
        /// </example>
        ConcurrentDictionary<string, object> Metadata { get; set; }

    }

}