using CloudNimble.SimpleMessageBus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// An <see cref="IMessageDispatcher"/> implementation that processes the messages in parallel, regardless of the order the <see cref="IMessageHandler">IMessageHandlers</see> 
    /// were registered with the Dependency Injection container.
    /// </summary>
    /// <remarks>
    /// This dispatcher invokes all matching message handlers concurrently using parallel execution. This provides
    /// better performance when handlers are independent and don't rely on execution order. However, it should be
    /// used carefully when handlers have side effects or shared dependencies that aren't thread-safe.
    /// </remarks>
    public class ParallelMessageDispatcher : IMessageDispatcher
    {

        #region Private Members

        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelMessageDispatcher"/> class.
        /// </summary>
        /// <param name="messageHandlers">The collection of message handlers to dispatch to.</param>
        public ParallelMessageDispatcher(IEnumerable<IMessageHandler> messageHandlers)
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
        /// Handlers are invoked concurrently using parallel execution. The method returns when all handlers
        /// have completed. If any handler throws an exception, other handlers will continue executing.
        /// </remarks>
        public async Task Dispatch(MessageEnvelope messageEnvelope)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(_messageHandlers.Where(c => c.GetHandledMessageTypes().Any(d => d.SimpleAssemblyQualifiedName() == messageEnvelope.MessageType)), handler =>
                {
                    _ = handler.OnNextAsync(messageEnvelope);
                });
            }).ConfigureAwait(false);
        }

        #endregion

    }

}