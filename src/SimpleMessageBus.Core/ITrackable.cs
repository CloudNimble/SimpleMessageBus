using System;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines a message that can track its parent for message lineage and correlation across the processing chain.
    /// </summary>
    /// <remarks>
    /// Message tracking enables building a complete audit trail of how messages flow through the system and
    /// which messages are related to each other. This is essential for debugging, monitoring, and understanding
    /// complex business processes that span multiple message types and handlers.
    /// 
    /// The ParentId property creates a direct parent-child relationship, while CorrelationId groups all related
    /// messages in the same business transaction or workflow, regardless of their hierarchical relationship.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Original message starts a workflow
    /// var orderCreated = new OrderCreatedMessage
    /// {
    ///     Id = Guid.NewGuid(),
    ///     CorrelationId = Guid.NewGuid(), // New workflow
    ///     ParentId = null, // No parent, this is the root
    ///     OrderNumber = "ORD-001"
    /// };
    /// 
    /// // Child message spawned during processing
    /// var inventoryReserved = new InventoryReservedMessage
    /// {
    ///     Id = Guid.NewGuid(),
    ///     CorrelationId = orderCreated.CorrelationId, // Same workflow
    ///     ParentId = orderCreated.Id, // Direct child of order created
    ///     ProductId = "PROD-123",
    ///     Quantity = 2
    /// };
    /// 
    /// // Another child in the same workflow
    /// var paymentProcessed = new PaymentProcessedMessage
    /// {
    ///     Id = Guid.NewGuid(),
    ///     CorrelationId = orderCreated.CorrelationId, // Same workflow  
    ///     ParentId = orderCreated.Id, // Also child of order created
    ///     Amount = 99.99m
    /// };
    /// </code>
    /// </example>
    public interface ITrackable
    {

        /// <summary>
        /// Gets or sets the ID of the parent message that triggered this message, enabling message lineage tracking.
        /// </summary>
        /// <value>
        /// A nullable <see cref="Guid"/> that identifies the immediate parent message. This value should be null
        /// for root messages that start a new workflow, and set to the parent's ID for derived or spawned messages.
        /// </value>
        /// <remarks>
        /// Use this property to build a hierarchical tree of message relationships. This enables tracing the exact
        /// sequence of message creation and understanding which message triggered which other messages. Combined with
        /// logging and monitoring, this provides powerful debugging and auditing capabilities.
        /// 
        /// When creating child messages in response to a parent message, always set this property to maintain the
        /// lineage chain.
        /// </remarks>
        /// <example>
        /// <code>
        /// // In a message handler creating child messages
        /// public async Task OnNextAsync(MessageEnvelope envelope)
        /// {
        ///     var parentMessage = envelope.Message;
        ///     
        ///     // Create child messages with proper lineage
        ///     var childMessage = new ChildMessage
        ///     {
        ///         ParentId = parentMessage.Id, // Link to parent
        ///         CorrelationId = (parentMessage as ITrackable)?.CorrelationId ?? Guid.NewGuid()
        ///     };
        ///     
        ///     await _publisher.PublishAsync(childMessage);
        /// }
        /// </code>
        /// </example>
        Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the correlation ID for tracking related messages across the entire processing chain.
        /// </summary>
        /// <value>
        /// A <see cref="Guid"/> that groups all messages belonging to the same business transaction or workflow.
        /// All related messages should share the same correlation ID regardless of their parent-child relationships.
        /// </value>
        /// <remarks>
        /// The correlation ID provides a way to group all messages that are part of the same logical operation
        /// or business process. Unlike ParentId which shows direct parent-child relationships, CorrelationId 
        /// creates a flat grouping that spans the entire workflow.
        /// 
        /// This is particularly useful for distributed tracing, log correlation, and understanding the full
        /// scope of a business operation that may spawn many parallel or sequential message processing paths.
        /// 
        /// For new workflows, generate a new correlation ID. For messages created in response to existing messages,
        /// inherit the correlation ID from the triggering message.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Starting a new workflow
        /// var rootMessage = new OrderCreatedMessage
        /// {
        ///     CorrelationId = Guid.NewGuid() // New workflow starts here
        /// };
        /// 
        /// // All subsequent messages inherit the same correlation ID
        /// var paymentMessage = new ProcessPaymentMessage
        /// {
        ///     CorrelationId = rootMessage.CorrelationId // Same workflow
        /// };
        /// 
        /// var shippingMessage = new ArrangeShippingMessage  
        /// {
        ///     CorrelationId = rootMessage.CorrelationId // Same workflow
        /// };
        /// 
        /// // Now all three messages can be correlated together in logs and monitoring
        /// </code>
        /// </example>
        Guid CorrelationId { get; set; }

    }

}