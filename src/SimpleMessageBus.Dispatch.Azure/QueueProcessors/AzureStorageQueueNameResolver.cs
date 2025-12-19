using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Helps dynamically resolve the name of the configured Queue at runtime.
    /// </summary>
    /// <remarks>See https://github.com/Azure/azure-webjobs-sdk/wiki/Queues#set-values-for-webjobs-sdk-constructor-parameters-in-code.</remarks>
    internal class AzureStorageQueueNameResolver : INameResolver
    {

        #region Private Members

        private AzureStorageQueueOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AzureStorageQueueNameResolver"/>
        /// </summary>
        /// <param name="options">The <see cref="IOptions{AzureQueueOptions}"/> instance injected from the DI container.</param>
        public AzureStorageQueueNameResolver(IOptions<AzureStorageQueueOptions> options)
        {
            _options = options.Value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves queue names for Azure WebJobs triggers and outputs based on configuration.
        /// </summary>
        /// <param name="name">The placeholder name from the WebJobs attribute to resolve.</param>
        /// <returns>The actual queue name from configuration.</returns>
        /// <remarks>
        /// This method maps WebJobs attribute placeholders to actual configured queue names,
        /// enabling dynamic queue name resolution at runtime.
        /// </remarks>
        public string Resolve(string name)
        {
            //RWM: This logic will have to be changed if we want to enable the ability to process more than one set of queues.
            return name == AzureStorageQueueConstants.CompletedQueueAttribute.Replace("%", "") ? _options.CompletedQueueName : _options.QueueName;
        }

        #endregion

    }

}