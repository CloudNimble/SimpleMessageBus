using CloudNimble.SimpleMessageBus.Core;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Dispatches messages to message handlers.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Dispatches the specified message envelope to message handlers.
        /// </summary>
        /// <param name="messageEnvelope">The message envelope to dispatch.</param>
        Task Dispatch(MessageEnvelope messageEnvelope);

    }

}