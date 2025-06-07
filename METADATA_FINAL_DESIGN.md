# Final Metadata Design with Interface Segregation

## Core Interfaces

### 1. Base IMessage Interface (Minimal)

```csharp
namespace CloudNimble.SimpleMessageBus.Core
{
    /// <summary>
    /// Defines the minimum implementation for a SimpleMessageBus message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// A unique identifier for the message.
        /// </summary>
        string Id { get; set; }
    }
}
```

### 2. IMetadataAware Interface

```csharp
using System.Collections.Concurrent;

namespace CloudNimble.SimpleMessageBus.Core
{
    /// <summary>
    /// Defines a message that supports metadata for passing data between handlers.
    /// </summary>
    public interface IMetadataAware
    {
        /// <summary>
        /// Thread-safe metadata storage for passing data between handlers in the processing pipeline.
        /// </summary>
        ConcurrentDictionary<string, object> Metadata { get; set; }
    }
}
```

### 3. ITrackableMessage Interface

```csharp
namespace CloudNimble.SimpleMessageBus.Core
{
    /// <summary>
    /// Defines a message that can track its parent for message lineage.
    /// </summary>
    public interface ITrackableMessage
    {
        /// <summary>
        /// The ID of the parent message that triggered this message, enabling message lineage tracking.
        /// </summary>
        string ParentId { get; set; }
        
        /// <summary>
        /// The correlation ID for tracking related messages across the entire processing chain.
        /// </summary>
        string CorrelationId { get; set; }
    }
}
```

### 4. MessageBase Abstract Class

```csharp
using System;
using System.Collections.Concurrent;

namespace CloudNimble.SimpleMessageBus.Core
{
    /// <summary>
    /// Base class providing a complete implementation of common message functionality.
    /// </summary>
    public abstract class MessageBase : IMessage, IMetadataAware, ITrackableMessage
    {
        #region Properties

        /// <inheritdoc />
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc />
        public ConcurrentDictionary<string, object> Metadata { get; set; } = new();

        /// <inheritdoc />
        public string ParentId { get; set; }

        /// <inheritdoc />
        public string CorrelationId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the message.
        /// </summary>
        protected MessageBase()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates a new instance of the message as a child of another message.
        /// </summary>
        /// <param name="parent">The parent message to inherit metadata from.</param>
        protected MessageBase(IMessage parent) : this()
        {
            ArgumentNullException.ThrowIfNull(parent);
            
            ParentId = parent.Id;
            
            // Inherit correlation ID if parent is trackable
            if (parent is ITrackableMessage trackableParent)
            {
                CorrelationId = trackableParent.CorrelationId;
            }
            
            // Inherit filtered metadata if parent has metadata
            if (parent is IMetadataAware metadataParent && metadataParent.Metadata != null)
            {
                Metadata = metadataParent.Metadata.Filter();
            }
        }

        #endregion
    }
}
```

### 5. Updated Extension Methods

```csharp
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
        /// <param name="handlerTypeName">The handler type name. If not provided, uses the calling type's name.</param>
        /// <returns><c>true</c> if this handler already ran successfully; otherwise, <c>false</c>.</returns>
        public static bool LastRunSucceeded(this IMessage message, [CallerMemberName] string handlerTypeName = null)
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
        /// <param name="handlerTypeName">The handler type name. If not provided, uses the calling type's name.</param>
        public static void UpdateResult(this IMessage message, bool status, [CallerMemberName] string handlerTypeName = null)
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
        public static TChild CreateChild<TChild>(this IMessage parent) where TChild : MessageBase, new()
        {
            return new TChild
            {
                ParentId = parent.Id,
                CorrelationId = parent is ITrackableMessage trackable ? trackable.CorrelationId : Guid.NewGuid().ToString(),
                Metadata = parent is IMetadataAware metadataParent ? metadataParent.Metadata?.Filter() : new()
            };
        }

        #endregion

        #region Private Methods

        private static string GetCallerTypeName()
        {
            var stackTrace = new StackTrace();
            
            // Skip frames to find the actual handler
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                var method = stackTrace.GetFrame(i)?.GetMethod();
                var declaringType = method?.DeclaringType;
                
                if (declaringType != null && 
                    !declaringType.Namespace?.StartsWith("System.") == true &&
                    !declaringType.Namespace?.StartsWith("Microsoft.") == true &&
                    typeof(IMessageHandler).IsAssignableFrom(declaringType))
                {
                    return declaringType.Name;
                }
            }
            
            return "UnknownHandler";
        }

        #endregion
    }
}
```

## Usage Patterns

### Idempotent Handler Pattern

```csharp
public class OrderValidationHandler : IMessageHandler<OrderCreated>
{
    public async Task Handle(OrderCreated message, ILogger logger)
    {
        // Check if this handler already processed this message successfully
        if (message.LastRunSucceeded(GetType().Name))
        {
            logger.LogInformation("Order {OrderId} already validated successfully", message.Id);
            return;
        }
        
        try
        {
            // Perform validation
            var isValid = await ValidateOrder(message);
            
            if (!isValid)
            {
                message.Metadata["ValidationErrors"] = GetValidationErrors();
                message.UpdateResult(false, GetType().Name);
                return;
            }
            
            // Add metadata for downstream handlers
            message.Metadata["ValidatedAt"] = DateTime.UtcNow;
            message.Metadata["CustomerTier"] = await GetCustomerTier(message.CustomerId);
            
            // Mark as successfully processed
            message.UpdateResult(true, GetType().Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Validation failed for order {OrderId}", message.Id);
            message.UpdateResult(false, GetType().Name);
            throw;
        }
    }
}
```

### Message Lineage Pattern

```csharp
public class OrderProcessingHandler : IMessageHandler<OrderCreated>
{
    private readonly IMessagePublisher _publisher;
    
    public async Task Handle(OrderCreated message, ILogger logger)
    {
        if (message.LastRunSucceeded(GetType().Name))
        {
            logger.LogInformation("Order {OrderId} already processed", message.Id);
            return;
        }
        
        // Process order
        var result = await ProcessOrder(message);
        
        if (result.Success)
        {
            // Create child event using constructor
            var orderProcessed = new OrderProcessed(message)
            {
                OrderId = message.OrderId,
                ProcessedAt = DateTime.UtcNow,
                TotalAmount = result.TotalAmount
            };
            
            // Or use extension method
            var shipmentRequested = message.CreateChild<ShipmentRequested>();
            shipmentRequested.OrderId = message.OrderId;
            shipmentRequested.Items = result.Items;
            
            await _publisher.PublishAsync(orderProcessed);
            await _publisher.PublishAsync(shipmentRequested);
            
            message.UpdateResult(true, GetType().Name);
        }
        else
        {
            message.UpdateResult(false, GetType().Name);
        }
    }
}
```

### Tracing Message Lineage

```csharp
public class MessageTracingHandler : IMessageHandler<IMessage>
{
    public async Task Handle(IMessage message, ILogger logger)
    {
        if (message is ITrackableMessage trackable)
        {
            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                ["MessageId"] = message.Id,
                ["ParentId"] = trackable.ParentId,
                ["CorrelationId"] = trackable.CorrelationId
            });
            
            logger.LogInformation("Processing message in correlation chain");
            
            // Can query all messages in a chain using CorrelationId
            // Can build parent-child tree using ParentId
        }
    }
}
```

## Migration Scenarios

### Minimal Migration (Just IMessage)

```csharp
// Existing message - no changes needed
public class LegacyMessage : IMessage
{
    public string Id { get; set; }
    public string Data { get; set; }
}
```

### Metadata-Enabled Migration

```csharp
// Before
public class OrderCreated : IMessage
{
    public string Id { get; set; }
    public string OrderId { get; set; }
}

// After - Option 1: Implement interfaces
public class OrderCreated : IMessage, IMetadataAware
{
    public string Id { get; set; }
    public ConcurrentDictionary<string, object> Metadata { get; set; } = new();
    public string OrderId { get; set; }
}

// After - Option 2: Inherit from MessageBase
public class OrderCreated : MessageBase
{
    public string OrderId { get; set; }
    
    // Constructor for root messages
    public OrderCreated() : base() { }
    
    // Constructor for child messages
    public OrderCreated(IMessage parent) : base(parent) { }
}
```

### Full-Featured Messages

```csharp
public class OrderProcessed : MessageBase
{
    public string OrderId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public decimal TotalAmount { get; set; }
    
    public OrderProcessed() : base() { }
    
    public OrderProcessed(IMessage parent) : base(parent) { }
}
```

## Benefits of This Design

1. **Interface Segregation** - Use only what you need
2. **Backward Compatible** - Existing IMessage implementations continue to work
3. **Idempotency Built-in** - Self-checking pattern prevents duplicate processing
4. **Message Lineage** - Track parent-child relationships and correlations
5. **Flexible Adoption** - Can implement interfaces individually or use MessageBase
6. **Clean Metadata Transfer** - Child messages inherit filtered parent metadata
7. **Loose Coupling** - Handlers don't need to know about each other

This design provides maximum flexibility while maintaining the magical simplicity!