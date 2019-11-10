using CloudNimble.SimpleMessageBus.Core;
using System;

namespace SimpleMessageBus.Tests.Shared
{

    /// <summary>
    /// 
    /// </summary>
    public class TestMessage : IMessage
    {

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        public TestMessage()
        {
            Id = Guid.NewGuid();
        }

    }

}
