using CloudNimble.SimpleMessageBus.Core;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Defines the required composition of every Dispatcher used by SimpleMessageBus to send <see cref="MessageEnvelope">MessageEnvelopes</see> to the 
    /// <see cref="IMessageHandler">IMessageHandlers</see> registered to handle that message's <see cref="System.Type"/>.
    /// </summary>
    /// <remarks>
    /// Message dispatchers control how messages are delivered to their handlers. SimpleMessageBus provides two built-in
    /// implementations: <see cref="OrderedMessageDispatcher"/> for sequential processing and <see cref="ParallelMessageDispatcher"/>
    /// for concurrent processing. Custom dispatchers can be implemented for specialized routing or processing logic.
    /// </remarks>
    public interface IMessageDispatcher
    {

        /// <summary>
        /// Dispatches an incoming <see cref="MessageEnvelope"/> to the <see cref="IMessageHandler">IMessageHandlers</see> registered to handle that message's <see cref="System.Type"/>.
        /// </summary>
        /// <param name="messageEnvelope">The <see cref="MessageEnvelope"/> instance to send to the registered <see cref="IMessageHandler">IMessageHandlers</see>.</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        /// <remarks>
        /// The implementation determines how handlers are invoked - sequentially, in parallel, or using custom logic.
        /// All matching handlers (those that declare support for the message type) will be called.
        /// </remarks>
        Task Dispatch(MessageEnvelope messageEnvelope);

    }

}