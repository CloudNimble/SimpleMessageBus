using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines the functionality required for all <see cref="IMessage"/> processing handlers.
    /// </summary>
    /// <remarks>
    /// Message handlers are the core processing units in SimpleMessageBus. They receive messages from queues,
    /// process them according to business logic, and handle any errors that occur during processing.
    /// Handlers can process multiple message types and are responsible for declaring which types they support.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class OrderMessageHandler : IMessageHandler
    /// {
    ///     private readonly IOrderService _orderService;
    ///     
    ///     public OrderMessageHandler(IOrderService orderService)
    ///     {
    ///         _orderService = orderService;
    ///     }
    ///     
    ///     public IEnumerable&lt;Type&gt; GetHandledMessageTypes()
    ///     {
    ///         yield return typeof(OrderCreatedMessage);
    ///         yield return typeof(OrderUpdatedMessage);
    ///     }
    ///     
    ///     public async Task OnNextAsync(MessageEnvelope messageEnvelope)
    ///     {
    ///         switch (messageEnvelope.Message)
    ///         {
    ///             case OrderCreatedMessage created:
    ///                 await _orderService.ProcessNewOrderAsync(created);
    ///                 break;
    ///             case OrderUpdatedMessage updated:
    ///                 await _orderService.UpdateOrderAsync(updated);
    ///                 break;
    ///         }
    ///     }
    ///     
    ///     public async Task OnErrorAsync(IMessage message, Exception exception)
    ///     {
    ///         // Log the error and potentially send notifications
    ///         await _orderService.HandleProcessingErrorAsync(message, exception);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IMessageHandler
    {
        /// <summary>
        /// Specifies which <see cref="IMessage"/> types are handled by this <see cref="IMessageHandler"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{Type}"/> containing all of the <see cref="IMessage"/> types this 
        /// <see cref="IMessageHandler"/> supports. The types must implement <see cref="IMessage"/>.
        /// </returns>
        /// <remarks>
        /// This method is called during handler registration to determine message routing. Return all message
        /// types that this handler can process. The framework will ensure that only messages of these types
        /// are delivered to this handler's <see cref="OnNextAsync"/> method.
        /// </remarks>
        /// <example>
        /// <code>
        /// public IEnumerable&lt;Type&gt; GetHandledMessageTypes()
        /// {
        ///     yield return typeof(OrderCreatedMessage);
        ///     yield return typeof(OrderCancelledMessage);
        ///     yield return typeof(OrderShippedMessage);
        /// }
        /// </code>
        /// </example>
        IEnumerable<Type> GetHandledMessageTypes();

        /// <summary>
        /// Specifies what this handler should do when an error occurs during processing.
        /// </summary>
        /// <param name="message">The deserialized <see cref="IMessage"/> instance that failed.</param>
        /// <param name="exception">The <see cref="Exception"/> that occurred during processing.</param> 
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        /// <remarks>
        /// This method is called when an exception is thrown during message processing. Use this method to
        /// implement custom error handling logic such as logging, alerting, or compensating transactions.
        /// Note that after this method completes, the message will typically be moved to a poison queue
        /// unless retry policies dictate otherwise.
        /// </remarks>
        /// <example>
        /// <code>
        /// public async Task OnErrorAsync(IMessage message, Exception exception)
        /// {
        ///     _logger.LogError(exception, "Failed to process {MessageType} with ID {MessageId}", 
        ///         message.GetType().Name, message.Id);
        ///     
        ///     // Send alert for critical messages
        ///     if (message is CriticalBusinessMessage)
        ///     {
        ///         await _alertService.SendFailureAlertAsync(message, exception);
        ///     }
        /// }
        /// </code>
        /// </example>
        Task OnErrorAsync(IMessage message, Exception exception);

        /// <summary>
        /// Specifies what this handler should do when it is time to process the <see cref="MessageEnvelope"/>.
        /// </summary>
        /// <param name="messageEnvelope">The <see cref="MessageEnvelope"/> to process.</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        /// <remarks>
        /// This is the main processing method for messages. The framework calls this method when a message
        /// of a supported type (as declared by <see cref="GetHandledMessageTypes"/>) is received from the queue.
        /// The message is pre-deserialized and available in the <see cref="MessageEnvelope.Message"/> property.
        /// Any unhandled exceptions thrown from this method will trigger a call to <see cref="OnErrorAsync"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// public async Task OnNextAsync(MessageEnvelope messageEnvelope)
        /// {
        ///     // Extract metadata if available
        ///     var userId = messageEnvelope.Metadata?.GetValueOrDefault("UserId");
        ///     
        ///     // Process based on message type
        ///     switch (messageEnvelope.Message)
        ///     {
        ///         case OrderCreatedMessage order:
        ///             await ProcessOrderAsync(order, userId?.ToString());
        ///             break;
        ///         default:
        ///             throw new NotSupportedException($"Message type {messageEnvelope.Message.GetType()} not supported");
        ///     }
        /// }
        /// </code>
        /// </example>
        Task OnNextAsync(MessageEnvelope messageEnvelope);

    }

}