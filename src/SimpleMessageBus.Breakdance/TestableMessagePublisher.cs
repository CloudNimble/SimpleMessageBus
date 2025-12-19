using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Breakdance
{

    /// <summary>
    /// A test double for IMessagePublisher that captures published messages for assertions.
    /// Used in testing scenarios to verify that expected messages were published correctly.
    /// </summary>
    /// <remarks>
    /// The TestableMessagePublisher is designed for unit and integration testing of message publishing scenarios.
    /// It implements the IMessagePublisher interface but instead of sending messages to actual queues, it captures
    /// them in memory for verification. This enables testing of message publishing behavior without requiring
    /// real message queue infrastructure.
    /// 
    /// Key testing features:
    /// - Captures all published messages for assertion
    /// - Supports configurable actions to simulate publish behavior
    /// - Provides methods to reset state between tests
    /// - Maintains message order for sequence verification
    /// 
    /// This class is part of the "Breakdance" testing utilities, named after the dance style that emphasizes
    /// breaking conventional patterns - just like how test doubles break the normal execution flow for testing.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic usage in a unit test
    /// var publisher = new TestableMessagePublisher();
    /// var messageHandler = new OrderHandler(publisher);
    /// 
    /// // Execute the code under test
    /// await messageHandler.ProcessOrder(orderId);
    /// 
    /// // Verify the expected messages were published
    /// Assert.AreEqual(2, publisher.PublishedMessages.Count);
    /// Assert.IsInstanceOfType(publisher.PublishedMessages[0], typeof(InventoryReservedMessage));
    /// Assert.IsInstanceOfType(publisher.PublishedMessages[1], typeof(PaymentProcessedMessage));
    /// 
    /// // Advanced scenario with custom action
    /// var publisher = new TestableMessagePublisher();
    /// publisher.SetAction((message, isSystem) => 
    /// {
    ///     // Simulate behavior like throwing exceptions for certain message types
    ///     if (message is ProblemMessage)
    ///         throw new InvalidOperationException("Simulated failure");
    /// });
    /// 
    /// // Test exception handling in your code
    /// await Assert.ThrowsExceptionAsync&lt;InvalidOperationException&gt;(() => 
    ///     messageHandler.ProcessProblemScenario());
    /// </code>
    /// </example>
    public class TestableMessagePublisher : IMessagePublisher
    {

        #region Fields

        private readonly List<IMessage> _publishedMessages = [];
        private Action<IMessage, bool> _onPublish;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a read-only list of all messages that have been published via this publisher.
        /// This collection can be used in test assertions to verify published message content.
        /// </summary>
        /// <value>
        /// A read-only list containing all messages published through this publisher in the order they were published.
        /// The collection is empty when the publisher is first created or after <see cref="ClearMessages"/> is called.
        /// </value>
        /// <remarks>
        /// This property provides access to all messages captured during testing. Messages are stored in publication
        /// order, allowing verification of both message content and sequence. The returned collection is read-only
        /// to prevent external modification of the test state.
        /// 
        /// Use this property in test assertions to verify:
        /// - The correct number of messages were published
        /// - The right message types were published
        /// - Messages contain expected data
        /// - Messages were published in the correct order
        /// </remarks>
        /// <example>
        /// <code>
        /// // Verify message count and types
        /// Assert.AreEqual(3, publisher.PublishedMessages.Count);
        /// Assert.IsTrue(publisher.PublishedMessages.Any(m => m is OrderCreatedMessage));
        /// 
        /// // Verify specific message content
        /// var orderMessage = publisher.PublishedMessages.OfType&lt;OrderCreatedMessage&gt;().First();
        /// Assert.AreEqual("ORD-001", orderMessage.OrderNumber);
        /// 
        /// // Verify message sequence
        /// Assert.IsInstanceOfType(publisher.PublishedMessages[0], typeof(OrderCreatedMessage));
        /// Assert.IsInstanceOfType(publisher.PublishedMessages[1], typeof(InventoryReservedMessage));
        /// Assert.IsInstanceOfType(publisher.PublishedMessages[2], typeof(PaymentProcessedMessage));
        /// </code>
        /// </example>
        public IReadOnlyList<IMessage> PublishedMessages => _publishedMessages.AsReadOnly();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestableMessagePublisher"/> class.
        /// Creates an empty publisher with no published messages or configured actions.
        /// </summary>
        public TestableMessagePublisher()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all published messages from the internal collection.
        /// Use this method to reset the state between tests.
        /// </summary>
        /// <remarks>
        /// This method is typically called in test setup or teardown to ensure each test starts with a clean state.
        /// After calling this method, the <see cref="PublishedMessages"/> collection will be empty until new messages
        /// are published. This prevents test interference where one test's published messages affect another test's assertions.
        /// </remarks>
        /// <example>
        /// <code>
        /// [Test]
        /// public async Task Should_Publish_Order_Message()
        /// {
        ///     // Arrange
        ///     publisher.ClearMessages(); // Ensure clean state
        ///     var order = new CreateOrderCommand { ProductId = "PROD-001" };
        ///     
        ///     // Act
        ///     await orderService.CreateOrderAsync(order);
        ///     
        ///     // Assert
        ///     Assert.AreEqual(1, publisher.PublishedMessages.Count);
        ///     Assert.IsInstanceOfType(publisher.PublishedMessages[0], typeof(OrderCreatedMessage));
        /// }
        /// </code>
        /// </example>
        public void ClearMessages()
        {
            _publishedMessages.Clear();
        }

        /// <summary>
        /// Sets an action to be executed when a message is published.
        /// </summary>
        /// <param name="onPublish">The action to execute when PublishAsync is called. 
        /// The action receives the published message and the isSystemGenerated flag.</param>
        /// <remarks>
        /// This method allows customization of the publisher's behavior during testing. The configured action
        /// is invoked after the message is added to the <see cref="PublishedMessages"/> collection, allowing
        /// for simulation of various publishing scenarios such as failures, delays, or side effects.
        /// 
        /// Setting this to null removes any previously configured action. The action is optional and publishing
        /// will work normally even without it being set.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Simulate publishing failures for certain message types
        /// publisher.SetAction((message, isSystem) =>
        /// {
        ///     if (message is CriticalMessage)
        ///         throw new InvalidOperationException("Publishing service unavailable");
        /// });
        /// 
        /// // Simulate side effects like logging or notifications
        /// publisher.SetAction((message, isSystem) =>
        /// {
        ///     if (isSystem)
        ///         systemMessageCount++;
        ///     logger.LogInformation("Published {MessageType}", message.GetType().Name);
        /// });
        /// 
        /// // Remove the action
        /// publisher.SetAction(null);
        /// </code>
        /// </example>
        public void SetAction(Action<IMessage, bool> onPublish)
        {
            _onPublish = onPublish;
        }

        /// <summary>
        /// Implements the IMessagePublisher interface method by capturing the published message
        /// for later assertion and optionally invoking a configured action.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <param name="isSystemGenerated">Indicates whether the message is system-generated.</param>
        /// <returns>A completed task.</returns>
        /// <remarks>
        /// This method implements the core functionality of the test double by:
        /// 1. Adding the message to the internal collection for later verification
        /// 2. Invoking any configured action (if set via <see cref="SetAction"/>)
        /// 3. Returning a completed task to satisfy the async interface
        /// 
        /// Unlike real message publishers, this method does not actually send messages to any queue.
        /// It completes synchronously and never throws exceptions unless a custom action is configured to do so.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Direct usage (though typically called by code under test)
        /// var message = new OrderCreatedMessage { OrderNumber = "ORD-001" };
        /// await publisher.PublishAsync(message, isSystemGenerated: false);
        /// 
        /// // Verify it was captured
        /// Assert.AreEqual(1, publisher.PublishedMessages.Count);
        /// Assert.AreSame(message, publisher.PublishedMessages[0]);
        /// </code>
        /// </example>
        /// <exception cref="Exception">May throw exceptions if a custom action configured via <see cref="SetAction"/> throws.</exception>
        public Task PublishAsync(IMessage message, bool isSystemGenerated = false)
        {
            _publishedMessages.Add(message);
            _onPublish?.Invoke(message, isSystemGenerated);
            return Task.CompletedTask;
        }

        #endregion

    }

}