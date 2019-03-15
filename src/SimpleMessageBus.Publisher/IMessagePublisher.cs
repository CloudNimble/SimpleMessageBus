using CloudNimble.SimpleMessageBus.Core;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Publish
{

    /// <summary>
    /// Defines the functionality required for publishing an <see cref="IMessage"/> to a supported backing queue.
    /// </summary>
    public interface IMessagePublisher
    {

        /// <summary>
        /// Publishes the specified <see cref="IMessage"/> to a queue.
        /// </summary>
        /// <param name="message">The message to add to the queue.</param>
        /// <param name="isSystemGenerated">
        /// Specifies whether the message comes from the currently-authenticated user, or is a system-generated emssage.
        /// </param>
        Task PublishAsync(IMessage message, bool isSystemGenerated = false);

    }

}