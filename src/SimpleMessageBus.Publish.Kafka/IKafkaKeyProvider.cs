using CloudNimble.SimpleMessageBus.Core;

namespace CloudNimble.SimpleMessageBus.Publish.Kafka
{

    /// <summary>
    /// Interface for providing Kafka message keys. Implement this on your <see cref="IMessage"/>
    /// types to control message partitioning in Kafka.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Kafka uses message keys to determine which partition a message goes to. Messages with
    /// the same key always go to the same partition, ensuring ordering for related messages.
    /// </para>
    /// <para>
    /// Common use cases:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Use a customer ID as key to ensure all events for a customer are processed in order</description></item>
    /// <item><description>Use an order ID to keep all order-related events together</description></item>
    /// <item><description>Use a tenant ID in multi-tenant systems</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class OrderUpdatedMessage : MessageBase, IKafkaKeyProvider
    /// {
    ///     public string OrderId { get; set; }
    ///     public string GetKafkaKey() => OrderId;
    /// }
    /// </code>
    /// </example>
    public interface IKafkaKeyProvider
    {

        /// <summary>
        /// Gets the Kafka message key for partition routing.
        /// </summary>
        /// <returns>
        /// A string key that determines partition assignment. Return <c>null</c> to use
        /// round-robin partitioning (no ordering guarantee).
        /// </returns>
        string GetKafkaKey();

    }

}
