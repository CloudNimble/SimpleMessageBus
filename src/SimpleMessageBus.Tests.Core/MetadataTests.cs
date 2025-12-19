using CloudNimble.SimpleMessageBus.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMessageBus.Samples.Core;
using System;
using System.Collections.Concurrent;

namespace SimpleMessageBus.Tests.Core
{

    /// <summary>
    /// Tests for the metadata functionality.
    /// </summary>
    [TestClass]
    public class MetadataTests
    {

        #region Test Methods

        /// <summary>
        /// Tests that MessageBase properly initializes with unique IDs and correlation IDs.
        /// </summary>
        [TestMethod]
        public void MessageBase_Should_Initialize_With_Unique_Ids()
        {
            var message1 = new NewUserMessage();
            var message2 = new NewUserMessage();

            message1.Id.Should().NotBe(Guid.Empty);
            message2.Id.Should().NotBe(Guid.Empty);
            message1.Id.Should().NotBe(message2.Id);

            message1.CorrelationId.Should().NotBe(Guid.Empty);
            message2.CorrelationId.Should().NotBe(Guid.Empty);
            message1.CorrelationId.Should().NotBe(message2.CorrelationId);

            message1.ParentId.Should().BeNull();
            message2.ParentId.Should().BeNull();

            message1.Metadata.Should().NotBeNull();
            message2.Metadata.Should().NotBeNull();
        }

        /// <summary>
        /// Tests that child messages inherit correlation ID and filtered metadata from parent.
        /// </summary>
        [TestMethod]
        public void MessageBase_Should_Inherit_From_Parent()
        {
            var parent = new NewUserMessage();
            parent.Metadata["UserTier"] = "Premium";
            parent.Metadata["ProcessingNode"] = "Node1";
            parent.Metadata["EmailSentHandler-Status"] = true;
            parent.Metadata["EmailSentHandler-Timestamp"] = DateTime.UtcNow;

            var child = new NewUserMessage(parent);

            child.Id.Should().NotBe(parent.Id);
            child.ParentId.Should().Be(parent.Id);
            child.CorrelationId.Should().Be(parent.CorrelationId);

            // Should inherit non-status metadata
            child.Metadata["UserTier"].Should().Be("Premium");
            child.Metadata["ProcessingNode"].Should().Be("Node1");

            // Should NOT inherit status metadata
            child.Metadata.Should().NotContainKey("EmailSentHandler-Status");
            child.Metadata.Should().NotContainKey("EmailSentHandler-Timestamp");
        }

        /// <summary>
        /// Tests the LastRunSucceeded and UpdateResult extension methods.
        /// </summary>
        [TestMethod]
        public void Message_Should_Track_Handler_Status()
        {
            var message = new NewUserMessage();
            var handlerName = "TestHandler";

            // Initially should return false
            message.LastRunSucceeded(handlerName).Should().BeFalse();

            // Update with success
            message.UpdateResult(true, handlerName);
            message.LastRunSucceeded(handlerName).Should().BeTrue();

            // Check metadata was set
            message.Metadata[$"{handlerName}-Status"].Should().Be(true);
            message.Metadata.Should().ContainKey($"{handlerName}-Timestamp");

            // Update with failure
            message.UpdateResult(false, handlerName);
            message.LastRunSucceeded(handlerName).Should().BeFalse();
            message.Metadata[$"{handlerName}-Status"].Should().Be(false);
        }

        /// <summary>
        /// Tests that non-metadata-aware messages handle extension methods gracefully.
        /// </summary>
        [TestMethod]
        public void NonMetadataAware_Message_Should_Handle_Extensions_Gracefully()
        {
            var message = new LegacyMessage { Id = Guid.NewGuid() };

            // LastRunSucceeded should return false for non-metadata-aware messages
            message.LastRunSucceeded("TestHandler").Should().BeFalse();

            // UpdateResult should throw for non-metadata-aware messages
            Action act = () => message.UpdateResult(true, "TestHandler");
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*must implement IMetadataAware*");
        }

        /// <summary>
        /// Tests the ConcurrentDictionary Filter extension method.
        /// </summary>
        [TestMethod]
        public void ConcurrentDictionary_Filter_Should_Remove_Status_Entries()
        {
            var metadata = new ConcurrentDictionary<string, object>();
            metadata["UserTier"] = "Premium";
            metadata["ProcessingNode"] = "Node1";
            metadata["Handler1-Status"] = true;
            metadata["Handler1-Timestamp"] = DateTime.UtcNow;
            metadata["Handler2-Status"] = false;
            metadata["Handler2-Timestamp"] = DateTime.UtcNow.AddMinutes(-5);

            var filtered = metadata.Filter();

            filtered.Should().HaveCount(2);
            filtered["UserTier"].Should().Be("Premium");
            filtered["ProcessingNode"].Should().Be("Node1");
            filtered.Should().NotContainKeys("Handler1-Status", "Handler1-Timestamp", "Handler2-Status", "Handler2-Timestamp");
        }

        /// <summary>
        /// Tests the ConcurrentDictionary FilterAndCombine extension method.
        /// </summary>
        [TestMethod]
        public void ConcurrentDictionary_FilterAndCombine_Should_Merge_Correctly()
        {
            var metadata = new ConcurrentDictionary<string, object>();
            metadata["UserTier"] = "Premium";
            metadata["ProcessingNode"] = "Node1";
            metadata["Handler1-Status"] = true;
            metadata["Handler1-Timestamp"] = DateTime.UtcNow;

            var payload = new System.Collections.Generic.Dictionary<string, object>
            {
                ["EventType"] = "UserCreated",
                ["UserTier"] = "Basic" // This should be overwritten by metadata
            };

            var result = metadata.FilterAndCombine(payload);

            result.Should().HaveCount(3);
            result["EventType"].Should().Be("UserCreated");
            result["UserTier"].Should().Be("Premium"); // Metadata value wins
            result["ProcessingNode"].Should().Be("Node1");
            result.Should().NotContainKeys("Handler1-Status", "Handler1-Timestamp");
        }

        /// <summary>
        /// Tests the CreateChild extension method.
        /// </summary>
        [TestMethod]
        public void CreateChild_Should_Work_With_Extension_Method()
        {
            var parent = new NewUserMessage();
            parent.Metadata["UserTier"] = "Premium";
            parent.Metadata["Handler1-Status"] = true;

            var child = parent.CreateChild<NewUserMessage>();

            child.Should().NotBeNull();
            child.Id.Should().NotBe(parent.Id);
            child.ParentId.Should().Be(parent.Id);
            child.CorrelationId.Should().Be(parent.CorrelationId);
            child.Metadata["UserTier"].Should().Be("Premium");
            child.Metadata.Should().NotContainKey("Handler1-Status");
        }

        #endregion

    }

    /// <summary>
    /// A legacy message that only implements IMessage (for testing backwards compatibility).
    /// </summary>
    public class LegacyMessage : IMessage
    {
        public Guid Id { get; set; }
    }

}