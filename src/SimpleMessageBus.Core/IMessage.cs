using System;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines the required composition of every Message published to the SimpleMessageBus.
    /// </summary>
    /// <remarks>
    /// All messages in the SimpleMessageBus system must implement this interface. The Id property ensures
    /// that each message can be uniquely identified throughout its lifecycle, including during processing,
    /// error handling, and when moved to poison queues.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class OrderCreatedMessage : IMessage
    /// {
    ///     public Guid Id { get; set; } = Guid.NewGuid();
    ///     public string OrderNumber { get; set; }
    ///     public decimal TotalAmount { get; set; }
    ///     public DateTime CreatedAt { get; set; }
    /// }
    /// </code>
    /// </example>
    public interface IMessage
    {

        /// <summary>
        /// Gets or sets the unique identifier for this Message.
        /// </summary>
        /// <value>
        /// A <see cref="Guid"/> that uniquely identifies this message instance. This value should be set when
        /// the message is created and remain constant throughout the message's lifetime.
        /// </value>
        /// <remarks>
        /// This identifier is used for tracking messages through the system, deduplication, and correlating
        /// messages in poison queues with their original instances.
        /// </remarks>
        Guid Id { get; set; }

    }

}