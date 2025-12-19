using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Represents a wrapper for an <see cref="IMessage"/> that will be published to the SimpleMessageBus Queue.
    /// </summary>
    /// <remarks>
    /// The MessageEnvelope provides the transport container for messages within the SimpleMessageBus system.
    /// It wraps the actual message with metadata needed for processing, error handling, and tracking. The envelope
    /// handles message serialization/deserialization and provides processing context for handlers.
    /// 
    /// Key responsibilities include:
    /// - Serializing messages for storage in queues
    /// - Tracking processing attempts and timestamps
    /// - Providing handler context through service scope and logging
    /// - Managing message state during processing pipeline
    /// - Facilitating message deserialization for handlers
    /// </remarks>
    /// <example>
    /// <code>
    /// // Creating an envelope (typically done automatically by publishers)
    /// var message = new OrderCreatedMessage { OrderNumber = "ORD-001" };
    /// var envelope = new MessageEnvelope(message);
    /// 
    /// // Processing an envelope in a handler
    /// public async Task OnNextAsync(MessageEnvelope envelope)
    /// {
    ///     // Access metadata
    ///     var attempts = envelope.AttemptsCount;
    ///     var published = envelope.DatePublished;
    ///     
    ///     // Extract the message
    ///     var orderMessage = envelope.GetMessage&lt;OrderCreatedMessage&gt;();
    ///     
    ///     // Use the logger for this specific message
    ///     envelope.ProcessLog?.LogInformation("Processing order {OrderNumber}", orderMessage.OrderNumber);
    ///     
    ///     // Process the message...
    /// }
    /// </code>
    /// </example>
    public class MessageEnvelope
    {

        #region Properties

        /// <summary>
        /// The number of times the system has previously attempted to process this message.
        /// </summary>
        public long AttemptsCount { get; set; }

        /// <summary>
        /// The UTC date and time that this nessage was published to the queue.
        /// </summary>
        public DateTimeOffset DatePublished { get; set; }

        /// <summary>
        /// A <see cref="Guid"/> uniquely identifying this message on the queue. Helps when looking at logs or correlating from telemetry. 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The serialized content of the <see cref="IMessage"/>.
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// A string representing the type name of the message. Defaults to IMessage.GetType().AssemblyQualifiedName".
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// A container to help track the state of a message as it flows between <see cref="IMessageHandler">IMessageHandlers</see>. This value is ignored by the 
        /// serializer and will not be persisted between failed message runs.
        /// </summary>
        [JsonIgnore]
        public dynamic MessageState { get; private set; }

        /// <summary>
        /// The processing log for this particular message across all MessageHandlers.
        /// </summary>
        [JsonIgnore]
        public ILogger ProcessLog { get; set; }

        /// <summary>
        /// The processing log for this particular message across all MessageHandlers.
        /// </summary>
        [JsonIgnore]
        public IServiceScope ServiceScope { get; set; }

        /// <summary>
        /// Gets the deserialized message instance.
        /// </summary>
        /// <value>
        /// The <see cref="IMessage"/> instance deserialized from <see cref="MessageContent"/> using the type specified
        /// in <see cref="MessageType"/>. This property provides convenient access to the message without requiring
        /// explicit type specification in handlers.
        /// </value>
        /// <remarks>
        /// This property deserializes the message on each access. For performance-critical scenarios where the message
        /// is accessed multiple times, consider caching the result or using the typed <see cref="GetMessage{T}"/> method.
        /// 
        /// The deserialization uses the <see cref="MessageType"/> to determine the target type and deserializes
        /// the <see cref="MessageContent"/> JSON string into the appropriate message instance.
        /// </remarks>
        /// <example>
        /// <code>
        /// public async Task OnNextAsync(MessageEnvelope envelope)
        /// {
        ///     // Type-safe access without specifying the type
        ///     switch (envelope.Message)
        ///     {
        ///         case OrderCreatedMessage order:
        ///             await ProcessOrderAsync(order);
        ///             break;
        ///         case PaymentProcessedMessage payment:
        ///             await ProcessPaymentAsync(payment);
        ///             break;
        ///         default:
        ///             throw new NotSupportedException($"Unknown message type: {envelope.MessageType}");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="InvalidOperationException">Thrown when the message type cannot be resolved or deserialization fails.</exception>
        [JsonIgnore]
        public IMessage Message
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MessageType) || string.IsNullOrWhiteSpace(MessageContent))
                {
                    throw new InvalidOperationException("MessageType and MessageContent must be set before accessing the Message property.");
                }

                var type = Type.GetType(MessageType);
                if (type is null)
                {
                    throw new InvalidOperationException($"Could not resolve message type '{MessageType}'. Ensure the assembly containing this type is loaded.");
                }

                try
                {
                    var message = JsonSerializer.Deserialize(MessageContent, type) as IMessage;
                    if (message is null)
                    {
                        throw new InvalidOperationException($"Deserialized object is not an IMessage. Type: {type.Name}");
                    }
                    return message;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"Failed to deserialize message content to type '{type.Name}'. Content: {MessageContent}", ex);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope"/> class.
        /// </summary>
        /// <remarks>
        /// This parameterless constructor should only be used for deserializing the MessageEnvelope from storage.
        /// For creating new envelopes to wrap messages, use the <see cref="MessageEnvelope(IMessage)"/> constructor instead.
        /// </remarks>
        public MessageEnvelope()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope"/> class for a given <see cref="IMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> instance that will be wrapped in a <see cref="MessageEnvelope"/> to be posted to the SimpleMessageBus.</param>
        /// <remarks>
        /// This constructor automatically serializes the message to JSON, generates a unique envelope ID,
        /// sets the publish timestamp to the current UTC time, and extracts the message type information
        /// for later deserialization. The resulting envelope is ready to be published to any queue provider.
        /// </remarks>
        /// <example>
        /// <code>
        /// var message = new OrderCreatedMessage 
        /// { 
        ///     OrderNumber = "ORD-001",
        ///     CustomerId = customerId,
        ///     TotalAmount = 99.99m
        /// };
        /// 
        /// var envelope = new MessageEnvelope(message);
        /// // envelope.Id is automatically generated
        /// // envelope.DatePublished is set to now
        /// // envelope.MessageType contains the full type name
        /// // envelope.MessageContent contains the JSON serialized message
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public MessageEnvelope(IMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Id = Guid.NewGuid();
            DatePublished = DateTimeOffset.UtcNow;
            MessageType = message.GetType().SimpleAssemblyQualifiedName();
            MessageContent = JsonSerializer.Serialize(message, message.GetType());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the <see cref="MessageContent" /> deserialized into an <see cref="IMessage"/> of the specified type.
        /// </summary>
        /// <typeparam name="T">The <see cref="IMessage"/> type represented by the <see cref="MessageContent"/>.</typeparam>
        /// <returns>A concrete <typeparamref name="T"/> instance populated with the data from the <see cref="MessageContent"/>.</returns>
        /// <remarks>
        /// This method provides type-safe deserialization when you know the exact message type at compile time.
        /// It directly deserializes the JSON content without using the <see cref="MessageType"/> property for type resolution.
        /// This can be more performant than the <see cref="Message"/> property for known types, but requires explicit type specification.
        /// </remarks>
        /// <example>
        /// <code>
        /// public async Task OnNextAsync(MessageEnvelope envelope)
        /// {
        ///     // Type-safe deserialization when you know the expected type
        ///     var orderMessage = envelope.GetMessage&lt;OrderCreatedMessage&gt;();
        ///     
        ///     // Process the strongly-typed message
        ///     await ProcessOrderAsync(orderMessage.OrderNumber, orderMessage.TotalAmount);
        /// }
        /// </code>
        /// </example>
        /// <exception cref="JsonException">Thrown when the <see cref="MessageContent"/> cannot be deserialized to type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="MessageContent"/> is null.</exception>
        public T GetMessage<T>() where T: IMessage
        {
            return JsonSerializer.Deserialize<T>(MessageContent);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        #endregion

    }

}