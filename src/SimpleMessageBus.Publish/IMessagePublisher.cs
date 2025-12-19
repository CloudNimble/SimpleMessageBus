using CloudNimble.SimpleMessageBus.Core;
using System;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Publish
{

    /// <summary>
    /// Defines the functionality required for publishing an <see cref="IMessage"/> to a supported backing queue.
    /// </summary>
    /// <remarks>
    /// Message publishers are the entry point for sending messages into the SimpleMessageBus system.
    /// Different implementations support various queue providers (Azure, Amazon, FileSystem, IndexedDB)
    /// while maintaining a consistent interface for application code.
    /// </remarks>
    public interface IMessagePublisher
    {

        /// <summary>
        /// Publishes the specified <see cref="IMessage"/> to a queue.
        /// </summary>
        /// <param name="message">The message to add to the queue.</param>
        /// <param name="isSystemGenerated">
        /// Specifies whether the message comes from the currently-authenticated user, or is a system-generated message.
        /// This flag can be used for auditing, routing, or processing decisions.
        /// </param>
        /// <returns>A task representing the asynchronous publish operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        Task PublishAsync(IMessage message, bool isSystemGenerated = false);

    }

}