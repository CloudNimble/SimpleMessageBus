//using Microsoft.Azure.Storage.Queue;
//using Newtonsoft.Json;

//namespace CloudNimble.SimpleMessageBus.Core
//{

//    /// <summary>
//    /// Extension methods to assist processing<see cref="MessageEnvelope">MessageEnvelopes</see>.
//    /// </summary>
//    public static class SimpleMessageBus_Publish_MessageEnvelopeExtensions
//    {

//        /// <summary>
//        /// Converts the <see cref="MessageEnvelope" /> into a <see cref="CloudQueueMessage" /> suitable for publishing to an Azure Storage Queue.
//        /// </summary>
//        /// <param name="messageEnvelope">The instance of the MessageEnvelop to convert.</param>
//        /// <returns>A <see cref="CloudQueueMessage"/> instance populated with the details from the <paramref name="messageEnvelope"/>.</returns>
//        public static CloudQueueMessage ToCloudQueueMessage(this MessageEnvelope messageEnvelope)
//        {
//            return new CloudQueueMessage(JsonConvert.SerializeObject(messageEnvelope));
//        }

//    }

//}