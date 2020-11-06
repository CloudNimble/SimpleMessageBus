using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Leverages the DI framework to make the injected <see cref="AzureStorageQueueOptions"/> available to the <see cref="QueuesOptions"/> to the default values can be set.
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
        /// The default constructor.
        /// </summary>
        /// <param name="azureStorageQueueOptions"></param>
        public QueuesOptionsConfiguration(IOptions<AzureStorageQueueOptions> azureStorageQueueOptions)
        {
            _azureStorageQueueOptions = azureStorageQueueOptions;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures a given <see cref="QueuesOptions"/> object with the passed-in values from <see cref="AzureStorageQueueOptions"/>.
        /// </summary>
        /// <param name="options"></param>
        public void Configure(QueuesOptions options)
        {
            options.BatchSize = _azureStorageQueueOptions.Value.ConcurrentJobs;
        }

        #endregion

    }

}