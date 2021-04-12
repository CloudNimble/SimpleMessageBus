using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        /// A string representing the type name of the message. Defaults to IMessage.GetType().AssemblyQualifiedName".
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
        [JsonIgnore]
        public ILogger ProcessLog { get; set; }

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
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            MessageType = message.GetType().SimpleAssemblyQualifiedName();
            MessageContent = JsonSerializer.Serialize(message);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the <see cref="MessageContent" /> deserialized into an <see cref="IMessage"/> of the specified type.
        /// </summary>
        /// <typeparam name="T">The <see cref="IMessage"/> type represented by the <see cref="MessageContent"/>.</typeparam>
        /// <returns>A concrete <typeparamref name="T"/> instance populated with the data from the <see cref="MessageContent"/>.</returns>
        public T GetMessage<T>() where T: IMessage
        {
            return JsonSerializer.Deserialize<T>(MessageContent);
        }

        #endregion

    }

}