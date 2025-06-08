using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Leverages the DI framework to make the injected <see cref="AzureStorageQueueOptions"/> available to the QueuesOptions to the default values can be set.
    /// </summary>
    /// <remarks>
    /// From https://benjamincollins.com/blog/using-dependency-injection-while-configuring-services/
    /// </remarks>
    internal class QueuesOptionsConfiguration : IConfigureOptions<QueuesOptions>
    {

        #region Private Members

        private readonly IOptions<AzureStorageQueueOptions> _azureStorageQueueOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuesOptionsConfiguration"/> class.
        /// </summary>
        /// <param name="azureStorageQueueOptions">The Azure Storage Queue options to use for configuration.</param>
        public QueuesOptionsConfiguration(IOptions<AzureStorageQueueOptions> azureStorageQueueOptions)
        {
            _azureStorageQueueOptions = azureStorageQueueOptions;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures a given QueuesOptions object with the passed-in values from <see cref="AzureStorageQueueOptions"/>.
        /// </summary>
        /// <param name="options">The QueuesOptions instance to configure.</param>
        public void Configure(QueuesOptions options)
        {
            options.BatchSize = _azureStorageQueueOptions.Value.ConcurrentJobs;
            options.MessageEncoding = (Azure.Storage.Queues.QueueMessageEncoding)(int)_azureStorageQueueOptions.Value.MessageEncoding;
        }

        #endregion

    }

}