using System;
using System.Collections.Concurrent;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Base class providing a complete implementation of common message functionality.
    /// </summary>
    /// <remarks>
    /// MessageBase provides a convenient base class that implements all core message interfaces, making it easy
    /// to create new message types without having to implement the common functionality manually. It automatically
    /// handles ID generation, metadata initialization, tracking properties, and provides constructors for creating
    /// child messages with proper lineage tracking.
    /// 
    /// This class is abstract and must be inherited to create concrete message types. It's recommended to use this
    /// base class for most message implementations unless you have specific requirements that prevent inheritance.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple message inheriting from MessageBase
    /// public class OrderCreatedMessage : MessageBase
    /// {
    ///     public string OrderNumber { get; set; }
    ///     public decimal TotalAmount { get; set; }
    ///     public DateTime CreatedAt { get; set; }
    /// }
    /// 
    /// // Creating a root message
    /// var orderMessage = new OrderCreatedMessage
    /// {
    ///     OrderNumber = "ORD-001",
    ///     TotalAmount = 99.99m,
    ///     CreatedAt = DateTime.UtcNow
    /// };
    /// 
    /// // Creating a child message with proper lineage
    /// public class PaymentProcessedMessage : MessageBase
    /// {
    ///     public decimal Amount { get; set; }
    ///     public string PaymentMethod { get; set; }
    ///     
    ///     public PaymentProcessedMessage(IMessage parent) : base(parent)
    ///     {
    ///         // Child-specific initialization
    ///     }
    /// }
    /// 
    /// var paymentMessage = new PaymentProcessedMessage(orderMessage)
    /// {
    ///     Amount = 99.99m,
    ///     PaymentMethod = "CreditCard"
    /// };
    /// // paymentMessage.ParentId == orderMessage.Id
    /// // paymentMessage.CorrelationId == orderMessage.CorrelationId
    /// </code>
    /// </example>
    public abstract class MessageBase : IMessage, IMetadataAware, ITrackable
    {

        #region Properties

        /// <inheritdoc />
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <inheritdoc />
        public ConcurrentDictionary<string, object> Metadata { get; set; } = new();

        /// <inheritdoc />
        public Guid? ParentId { get; set; }

        /// <inheritdoc />
        public Guid CorrelationId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the message with automatically generated ID and correlation ID.
        /// </summary>
        /// <remarks>
        /// This constructor is used for creating root messages that start new workflows. It automatically
        /// generates a new message ID and a new correlation ID, and initializes the metadata dictionary.
        /// The ParentId is left null, indicating this is a root message in the processing chain.
        /// </remarks>
        protected MessageBase()
        {
            CorrelationId = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new instance of the message as a child of another message.
        /// </summary>
        /// <param name="parent">The parent message to inherit metadata from.</param>
        /// <remarks>
        /// This constructor is used for creating child messages that are spawned during the processing of
        /// a parent message. It automatically sets up the proper parent-child relationship by setting ParentId
        /// to the parent's ID, inherits the correlation ID from trackable parents, and copies filtered metadata
        /// from metadata-aware parents.
        /// 
        /// The metadata filtering ensures that only relevant metadata is inherited by child messages, preventing
        /// unbounded growth of metadata collections in long processing chains.
        /// </remarks>
        /// <example>
        /// <code>
        /// // In a message handler, create child messages with proper lineage
        /// public async Task OnNextAsync(MessageEnvelope envelope)
        /// {
        ///     var orderMessage = envelope.Message as OrderCreatedMessage;
        ///     
        ///     // Create child message inheriting from parent
        ///     var inventoryMessage = new ReserveInventoryMessage(orderMessage)
        ///     {
        ///         ProductId = orderMessage.ProductId,
        ///         Quantity = orderMessage.Quantity
        ///     };
        ///     
        ///     await _publisher.PublishAsync(inventoryMessage);
        ///     // inventoryMessage.ParentId == orderMessage.Id
        ///     // inventoryMessage.CorrelationId == orderMessage.CorrelationId
        ///     // inventoryMessage.Metadata contains filtered parent metadata
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> is null.</exception>
        protected MessageBase(IMessage parent) : this()
        {
            ArgumentNullException.ThrowIfNull(parent);

            ParentId = parent.Id;

            // Inherit correlation ID if parent is trackable
            if (parent is ITrackable trackableParent)
            {
                CorrelationId = trackableParent.CorrelationId;
            }

            // Inherit filtered metadata if parent has metadata
            if (parent is IMetadataAware metadataParent && metadataParent.Metadata is not null)
            {
                Metadata = metadataParent.Metadata.Filter();
            }
        }

        #endregion

    }

}