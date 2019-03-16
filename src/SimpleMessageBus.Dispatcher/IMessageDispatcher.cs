using CloudNimble.SimpleMessageBus.Core;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Defines the required composition of every Dispatcher used by SimpleMessageBus to send <see cref="MessageEnvelope">MessageEnvelopes</see> to the 
    /// <see cref="IMessageHandler">IMessageHandlers</see> registered to handle that message's <see cref="System.Type"/>.
    /// </summary>
    public interface IMessageDispatcher
    {

        /// <summary>
        /// Dispatches an incoming <see cref="MessageEnvelope"/> to the <see cref="IMessageHandler">IMessageHandlers</see> registered to handle that message's <see cref="System.Type"/>.
        /// </summary>
        /// <param name="messageEnvelope">The <see cref="MessageEnvelope"/> instance to send to the registered <see cref="IMessageHandler">IMessageHandlers</see>.</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        Task Dispatch(MessageEnvelope messageEnvelope);

    }

}