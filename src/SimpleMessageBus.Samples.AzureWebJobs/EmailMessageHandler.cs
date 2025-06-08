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
        /// Processes the message envelope, demonstrating the new idempotency pattern.
        /// </summary>
        /// <param name="messageEnvelope">The message envelope to process.</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        public async Task OnNextAsync(MessageEnvelope messageEnvelope)
        {
            var result = false;
            var messageType = Type.GetType(messageEnvelope.MessageType);

            switch (messageType)
            {
                case Type newUserMessage when newUserMessage == typeof(NewUserMessage):
                    var message = messageEnvelope.GetMessage<NewUserMessage>();
                    
                    // Check if this handler already processed this message successfully
                    if (message.LastRunSucceeded(GetType().Name))
                    {
                        Console.WriteLine($"Message {message.Id} already processed successfully by {GetType().Name}");
                        return;
                    }
                    
                    try
                    {
                        result = await SendNewUserEmailAsync(message);
                        message.UpdateResult(result, GetType().Name);
                        
                        // Add some metadata for demonstration
                        if (result)
                        {
                            message.Metadata["EmailSentAt"] = DateTime.UtcNow;
                            message.Metadata["ProcessedBy"] = Environment.MachineName;
                        }
                    }
                    catch (Exception ex)
                    {
                        message.UpdateResult(false, GetType().Name);
                        message.Metadata["LastError"] = ex.Message;
                        throw;
                    }
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