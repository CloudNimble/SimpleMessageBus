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
        public void ClearMessages()
        {
            _publishedMessages.Clear();
        }

        /// <summary>
        /// Sets an action to be executed when a message is published.
        /// </summary>
        /// <param name="onPublish">The action to execute when PublishAsync is called. 
        /// The action receives the published message and the isSystemGenerated flag.</param>
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
        public Task PublishAsync(IMessage message, bool isSystemGenerated = false)
        {
            _publishedMessages.Add(message);
            _onPublish?.Invoke(message, isSystemGenerated);
            return Task.CompletedTask;
        }

        #endregion

    }

}