using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using CloudNimble.SimpleMessageBus.Dispatch.Kafka;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleMessageBus.Tests.Dispatch.Kafka
{

    /// <summary>
    /// Integration tests for Kafka processor.
    /// </summary>
    /// <remarks>
    /// These tests require a running Kafka instance. Use Testcontainers.Kafka
    /// to spin up a container, or configure connection to an existing cluster.
    /// </remarks>
    [TestClass]
    public class KafkaProcessorTests
    {

        // TODO: Implement integration tests using Testcontainers.Kafka
        // when Docker-based testing infrastructure is available.
        //
        // Test scenarios to implement:
        // 1. MessagePublisher_PublishesMessage_ToKafkaTopic
        // 2. KafkaProcessor_ProcessesMessage_DispatchesToHandler
        // 3. KafkaProcessor_WithKey_RoutesToCorrectPartition
        // 4. KafkaProcessor_OnFailure_DoesNotCommitOffset

        /// <summary>
        /// Verifies that <see cref="KafkaProcessor"/> implements <see cref="IQueueProcessor"/>.
        /// </summary>
        [TestMethod]
        public void KafkaProcessor_ImplementsIQueueProcessor()
        {
            typeof(KafkaProcessor).Should().Implement<IQueueProcessor>();
        }

    }

}
