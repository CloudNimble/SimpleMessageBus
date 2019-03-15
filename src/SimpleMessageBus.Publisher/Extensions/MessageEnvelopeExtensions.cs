using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Extension methods for the <see cref="MessageEnvelope{T}"/> object.
    /// </summary>
    public static class MessageEnvelopeExtensions
    {

        /// <summary>
        /// Converts the MessageEnvelope into a CloudQueueMessage suitable for publishing to an Azure Storage Queue.
        /// </summary>
        /// <typeparam name="T">The type of the Entity to be published.</typeparam>
        /// <param name="messageEnvelope">The instance of the MessageEnvelop to convert.</param>
        /// <returns></returns>
        public static CloudQueueMessage ToCloudQueueMessage(this MessageEnvelope messageEnvelope)
        {
            return new CloudQueueMessage(JsonConvert.SerializeObject(messageEnvelope));
        }

    }

}