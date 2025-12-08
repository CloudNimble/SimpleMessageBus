using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch.Kafka
{

    /// <summary>
    /// Resolves Kafka configuration placeholders at runtime for WebJobs triggers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This resolver maps placeholders in WebJobs attributes (like "%topic%") to actual
    /// configuration values from <see cref="KafkaOptions"/>.
    /// </para>
    /// <para>
    /// See https://github.com/Azure/azure-webjobs-sdk/wiki/Queues#set-values-for-webjobs-sdk-constructor-parameters-in-code
    /// for more information about name resolution in WebJobs.
    /// </para>
    /// </remarks>
    internal class KafkaNameResolver : INameResolver
    {

        #region Private Members

        private readonly KafkaOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="KafkaNameResolver"/>.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{KafkaOptions}"/> instance injected from the DI container.</param>
        public KafkaNameResolver(IOptions<KafkaOptions> options)
        {
            _options = options.Value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves a placeholder name to its configured value.
        /// </summary>
        /// <param name="name">The placeholder name (without % delimiters).</param>
        /// <returns>The resolved configuration value, or null if the placeholder is not recognized.</returns>
        /// <remarks>
        /// Supported placeholders:
        /// <list type="bullet">
        /// <item><description><c>topic</c> - resolves to <see cref="KafkaOptions.TopicName"/></description></item>
        /// <item><description><c>consumergroup</c> - resolves to <see cref="KafkaOptions.ConsumerGroup"/></description></item>
        /// <item><description><c>brokerlist</c> - resolves to <see cref="KafkaOptions.BrokerList"/></description></item>
        /// </list>
        /// </remarks>
        public string Resolve(string name)
        {
            return name switch
            {
                "topic" => _options.TopicName,
                "consumergroup" => _options.ConsumerGroup,
                "brokerlist" => _options.BrokerList,
                _ => null
            };
        }

        #endregion

    }

}
