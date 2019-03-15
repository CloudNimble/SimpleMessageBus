using Newtonsoft.Json;
using System;
using System.IO;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Represents a wrapper for an <see cref="IMessage"/> that will be published to the SimpleMessageBus Queue.
    /// </summary>
    public class MessageEnvelope
    {

        #region Properties

        /// <summary>
        /// The number of times the system has previously attempted to process this message.
        /// </summary>
        public int AttemptsCount { get; set; }

        /// <summary>
        /// The serialized content of the <see cref="IMessage"/>.
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// A string representing the type name of the message, usually represented by "IMessage.GetType().Name".
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The UTC date and time that this nessage was published to the queue.
        /// </summary>
        public DateTimeOffset DatePublished { get; set; }

        /// <summary>
        /// A <see cref="Guid"/> uniquely identifying this message on the queue. Helps when looking at logs or correlating from telemetry. 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The processing log for this particular message across all MessageHandlers.
        /// </summary>
        public TextWriter ProcessLog { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor. Should only be used for deserializing the MessageEnvelope. For other uses, please use <see cref="MessageEnvelope(IMessage)"/> instead.
        /// </summary>
        public MessageEnvelope()
        {
        }

        /// <summary>
        /// Creates a new <see cref="MessageEnvelope"/> instance for a given <see cref="IMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> instance that will be wrapped in a <see cref="MessageEnvelope"/> to be posted to the SimpleMessageBus.</param>
        public MessageEnvelope(IMessage message)
        {
            MessageType = message.GetType().Name;
            MessageContent = JsonConvert.SerializeObject(message);
        }

        #endregion

    }

}