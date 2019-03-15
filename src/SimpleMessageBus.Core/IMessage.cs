using System;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Defines the required composition of every Message published to the SimpleMessageBus.
    /// </summary>
    public interface IMessage
    {

        /// <summary>
        /// The unique identifier for this Message.
        /// </summary>
        Guid Id { get; set; }

    }

}