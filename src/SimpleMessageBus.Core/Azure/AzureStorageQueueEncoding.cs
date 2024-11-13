namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Determines how QueueMessage.Body is represented in HTTP requests and responses.
    /// </summary>
    public enum AzureStorageQueueEncoding
    {

        /// <summary>
        /// The QueueMessage.Body is represented verbatim in HTTP requests and responses. I.e. message is not transformed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The QueueMessage.Body is represented as Base64 encoded string in HTTP requests and responses.
        /// </summary>
        /// <remarks>
        /// This was the default behavior in the prior v11 library.  See
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.storage.queue.cloudqueue.encodemessage?view=azure-dotnet-legacy">CloudQueue.EncodeMessage</see>.
        /// Using this option can make interop with an existing application easier.
        /// </remarks>
        Base64 = 1,

    }

}
