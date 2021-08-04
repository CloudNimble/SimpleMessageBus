using CloudNimble.SimpleMessageBus.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMessageBus.Tests.Shared;

namespace SimpleMessageBus.Tests.Core
{
    [TestClass]
    public class MessageEnvelopTests
    {
        [TestMethod]
        public void MessageEnvelop_SerializesMessageContent()
        {
            var testMessage = new TestMessage
            {
                Greeting = "Hello world!",
                PickANumber = 42,
                BirthDay = new System.DateTime(1970, 4, 14)
            };
            testMessage.Id.Should().NotBeEmpty();

            var envelope = new MessageEnvelope(testMessage);

            envelope.MessageContent.Should().NotBeNullOrEmpty();
            envelope.MessageContent.Should().ContainAll(testMessage.Id.ToString(), testMessage.Greeting, testMessage.PickANumber.ToString(), testMessage.BirthDay.ToString("yyyy-MM-dd"));
            envelope.MessageType.Should().Contain(testMessage.GetType().FullName);
        }

    }
}
