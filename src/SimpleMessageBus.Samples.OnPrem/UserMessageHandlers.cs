using CloudNimble.SimpleMessageBus.Core;
using SimpleMessageBus.Samples.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleMessageBus.Samples.OnPrem
{

    /// <summary>
    /// 
    /// </summary>
    public class TestMessageHandler : IMessageHandler
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        public Task OnNextAsync(MessageEnvelope messageEnvelope)
        {
            return Task.Run(() => Console.WriteLine("HAHAHAHAHAHAHA"));
        }


    }

}