using CloudNimble.SimpleMessageBus.Core;
using System;

namespace SimpleMessageBus.Samples.Core
{

    /// <summary>
    /// An <see cref="IMessage"/> signifying that a new user has registered with the system.
    /// </summary>
    /// <remarks>
    /// This is the kind of message that would be typical with any app. When a new user registers, you might want to do something like:
    /// 1) Register a conversion in your SaaS metrics app.
    /// 2) Send them a welcome email.
    /// 3) Send yourself a test message letting you know you have a new signup.
    /// </remarks>
    public class NewUserMessage : MessageBase
    {

        #region Properties

        /// <summary>
        /// The email address of the user that registered.
        /// </summary>
        public string Email { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="NewUserMessage"/>.
        /// </summary>
        public NewUserMessage() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NewUserMessage"/> as a child of another message.
        /// </summary>
        /// <param name="parent">The parent message to inherit metadata from.</param>
        public NewUserMessage(IMessage parent) : base(parent)
        {
        }

        #endregion
               
    }

}