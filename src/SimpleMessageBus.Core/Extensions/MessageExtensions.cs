using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Extension methods for IMessage operations.
    /// </summary>
    public static class MessageExtensions
    {

        #region Public Methods

        /// <summary>
        /// Checks if the current handler has already successfully processed this message.
        /// This enables idempotent message processing.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <param name="handlerTypeName">The handler type name. If not provided, attempts to detect from call stack.</param>
        /// <returns><c>true</c> if this handler already ran successfully; otherwise, <c>false</c>.</returns>
        /// <example>
        /// <code>
        /// public async Task Handle(OrderCreated message, ILogger logger)
        /// {
        ///     if (message.LastRunSucceeded(GetType().Name))
        ///     {
        ///         logger.LogInformation("Order {OrderId} already processed successfully", message.Id);
        ///         return;
        ///     }
        ///     
        ///     // Process the message...
        ///     message.UpdateResult(true, GetType().Name);
        /// }
        /// </code>
        /// </example>
        public static bool LastRunSucceeded(this IMessage message, string handlerTypeName = null)
        {
            ArgumentNullException.ThrowIfNull(message);

            if (message is not IMetadataAware metadataAware)
            {
                return false;
            }

            metadataAware.Metadata ??= new ConcurrentDictionary<string, object>();

            // If called with GetType().Name, use it directly
            // Otherwise, try to detect the handler name
            handlerTypeName ??= GetCallerTypeName();

            return metadataAware.Metadata.TryGetValue($"{handlerTypeName}-Status", out var status) &&
                   status is bool succeeded && succeeded;
        }

        /// <summary>
        /// Updates the execution status for the current handler.
        /// </summary>
        /// <param name="message">The message to update.</param>
        /// <param name="status">The execution status to record.</param>
        /// <param name="handlerTypeName">The handler type name. If not provided, attempts to detect from call stack.</param>
        /// <example>
        /// <code>
        /// public async Task Handle(OrderCreated message, ILogger logger)
        /// {
        ///     try
        ///     {
        ///         await ProcessOrder(message);
        ///         message.UpdateResult(true, GetType().Name);
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         logger.LogError(ex, "Failed to process order");
        ///         message.UpdateResult(false, GetType().Name);
        ///         throw;
        ///     }
        /// }
        /// </code>
        /// </example>
        public static void UpdateResult(this IMessage message, bool status, string handlerTypeName = null)
        {
            ArgumentNullException.ThrowIfNull(message);

            if (message is not IMetadataAware metadataAware)
            {
                throw new InvalidOperationException($"Message of type {message.GetType().Name} must implement IMetadataAware to use UpdateResult.");
            }

            metadataAware.Metadata ??= new ConcurrentDictionary<string, object>();

            handlerTypeName ??= GetCallerTypeName();

            metadataAware.Metadata.AddOrUpdate($"{handlerTypeName}-Status", status, (key, oldValue) => status);
            metadataAware.Metadata.AddOrUpdate($"{handlerTypeName}-Timestamp", DateTime.UtcNow, (key, oldValue) => DateTime.UtcNow);
        }

        /// <summary>
        /// Creates a child message that inherits metadata and correlation from the parent.
        /// </summary>
        /// <typeparam name="TChild">The type of child message to create.</typeparam>
        /// <param name="parent">The parent message.</param>
        /// <returns>A new child message with inherited metadata and correlation.</returns>
        /// <example>
        /// <code>
        /// // Create a child event that inherits filtered parent metadata
        /// var shipmentRequested = message.CreateChild&lt;ShipmentRequested&gt;();
        /// shipmentRequested.OrderId = message.OrderId;
        /// shipmentRequested.Items = result.Items;
        /// 
        /// await publisher.PublishAsync(shipmentRequested);
        /// </code>
        /// </example>
        public static TChild CreateChild<TChild>(this IMessage parent) where TChild : MessageBase, new()
        {
            ArgumentNullException.ThrowIfNull(parent);

            var child = new TChild
            {
                ParentId = parent.Id,
                CorrelationId = parent is ITrackable trackable ? trackable.CorrelationId : Guid.NewGuid(),
                Metadata = parent is IMetadataAware metadataParent && metadataParent.Metadata is not null 
                    ? metadataParent.Metadata.Filter() 
                    : new ConcurrentDictionary<string, object>()
            };

            return child;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the calling type name using stack frame inspection.
        /// </summary>
        private static string GetCallerTypeName()
        {
            var stackTrace = new StackTrace();

            // Skip frames to find the actual handler
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                var method = stackTrace.GetFrame(i)?.GetMethod();
                var declaringType = method?.DeclaringType;

                if (declaringType is not null &&
                    !declaringType.Namespace?.StartsWith("System.") == true &&
                    !declaringType.Namespace?.StartsWith("Microsoft.") == true)
                {
                    // Try to find if this type implements IMessageHandler
                    var interfaces = declaringType.GetInterfaces();
                    foreach (var iface in interfaces)
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition().Name.Contains("IMessageHandler"))
                        {
                            return declaringType.Name;
                        }
                    }
                }
            }

            return "UnknownHandler";
        }

        #endregion

    }

}