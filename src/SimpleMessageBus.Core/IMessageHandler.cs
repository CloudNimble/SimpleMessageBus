using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines the functionality required for all <see cref="IMessage"/> processing handlers.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Specifies which <see cref="IMessage"/> types are handled by this <see cref="IMessageHandler"/>.
        /// </summary>
        IEnumerable<Type> GetHandledMessageTypes();

        /// <summary>
        /// Specifies what this handler should do when an error occurs during processing.
        /// </summary>
        /// <param name="message">The deserialized <see cref="IMessage"/> instance that failed.</param>
        /// <param name="exception">The <see cref="Exception"/> that occurred during processing.</param> 
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        Task OnErrorAsync(IMessage message, Exception exception);

        /// <summary>
        /// Specifies what this handler should do when it is time to process the <see cref="MessageEnvelope"/>.
        /// </summary>
        /// <param name="messageEnvelope">The <see cref="MessageEnvelope"/> to process.</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        Task OnNextAsync(MessageEnvelope messageEnvelope);

    }

}