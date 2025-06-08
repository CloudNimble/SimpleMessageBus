using CloudNimble.SimpleMessageBus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// An <see cref="IMessageDispatcher"/> implementation that processes the messages in the order the <see cref="IMessageHandler">IMessageHandlers</see> 
    /// were registered with the Dependency Injection container.
    /// </summary>
    /// <remarks>
    /// This dispatcher ensures that message handlers are invoked sequentially in registration order. This is useful
    /// when handler execution order matters, such as when one handler's output affects another handler's behavior.
    /// Each handler completes before the next one begins, providing predictable execution flow but potentially
    /// slower overall processing compared to parallel dispatching.
    /// </remarks>
    public class OrderedMessageDispatcher : IMessageDispatcher
    {

        #region Private Members

        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedMessageDispatcher"/> class.
        /// </summary>
        /// <param name="messageHandlers">The collection of message handlers to dispatch to.</param>
        public OrderedMessageDispatcher(IEnumerable<IMessageHandler> messageHandlers)
        {
            _messageHandlers = messageHandlers;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the <see cref="MessageEnvelope"/> to the <see cref="IMessageHandler">MessageHandlers</see> registered to that type, for processing.  
        /// </summary>
        /// <param name="messageEnvelope">The <see cref="MessageEnvelope"/> instance to be processed.</param>
        /// <remarks>
        /// Handlers are invoked sequentially in the order they were registered with the DI container.
        /// If any handler throws an exception, subsequent handlers will not be invoked.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageEnvelope"/> is null.</exception>
        public async Task Dispatch(MessageEnvelope messageEnvelope)
        {
            if (messageEnvelope is null)
            {
                throw new ArgumentNullException(nameof(messageEnvelope));
            }

            foreach (var handler in _messageHandlers.Where(c => c.GetHandledMessageTypes().Any(d => d.SimpleAssemblyQualifiedName() == messageEnvelope.MessageType)))
            {
                await handler.OnNextAsync(messageEnvelope).ConfigureAwait(false);
            }
        }

        #endregion

    }

}