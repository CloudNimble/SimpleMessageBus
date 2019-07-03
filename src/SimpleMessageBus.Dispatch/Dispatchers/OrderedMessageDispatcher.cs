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
    public class OrderedMessageDispatcher : IMessageDispatcher
    {

        #region Private Members

        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageHandlers"></param>
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
        public async Task Dispatch(MessageEnvelope messageEnvelope)
        {
            if (messageEnvelope == null)
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