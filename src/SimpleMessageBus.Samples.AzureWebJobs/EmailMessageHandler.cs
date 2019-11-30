using CloudNimble.SimpleMessageBus.Core;
using SimpleMessageBus.Samples.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleMessageBus.Samples.AzureWebJobs
{

    /// <summary>
    /// An example handler that shows how to process <see cref="IMessage">IMessages</see> coming off the queue that require email notifications.
    /// </summary>
    public class EmailMessageHandler : IMessageHandler
    {

        /// <summary>
        /// Specifies which <see cref="IMessage"/> types are handled by this <see cref="IMessageHandler"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Type}"/> containing all of the <see cref="IMessage"/> types this <see cref="IMessageHandler"/> supports.</returns>
        public IEnumerable<Type> GetHandledMessageTypes()
        {
            yield return typeof(NewUserMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task OnErrorAsync(IMessage message, Exception exception) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageEnvelope"></param>
        /// <returns></returns>
        public async Task OnNextAsync(MessageEnvelope messageEnvelope)
        {
            var result = false;
            var messageType = Type.GetType(messageEnvelope.MessageType);

            switch (messageType)
            {
                case Type newUserMessage when newUserMessage == typeof(NewUserMessage):
                    result = await SendNewUserEmailAsync(messageEnvelope.GetMessage<NewUserMessage>());
                    break;
            }

            //RWM: Throw an exception to get the message tossed in the poison queue.
            if (!result)
            {
                throw new Exception("Message processing failed.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newUserMessage"></param>
        /// <returns></returns>
        internal async Task<bool> SendNewUserEmailAsync(NewUserMessage newUserMessage)
        {
            //TODO: Send email here.
            Console.WriteLine("The message has been processed.");
            return await Task.FromResult(true);
        }

    }

}