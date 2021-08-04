using CloudNimble.SimpleMessageBus.Core;
using System;

namespace SimpleMessageBus.Tests.Shared
{

    /// <summary>
    /// A test message for the publishers / dispatchers.
    /// </summary>
    public class TestMessage : IMessage
    {

        /// <summary>
        /// Message identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Default constructor that sets a unique value for the identifier.
        /// </summary>
        public TestMessage()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// String value to test that all properties are being serialized correctly.
        /// </summary>
        public string Greeting { get; set; }

        /// <summary>
        /// Integer value to test that all properties are being serialized correctly.
        /// </summary>
        public int PickANumber { get; set; }

        /// <summary>
        /// DateTime value to test that all properties are being serialized correctly.
        /// </summary>
        public DateTime BirthDay { get; set; }

    }
}
