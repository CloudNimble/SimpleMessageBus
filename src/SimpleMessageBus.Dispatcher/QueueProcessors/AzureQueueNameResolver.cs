using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Helps dynamically resolve the name of the configured Queue at runtime.
    /// </summary>
    /// <remarks>See https://github.com/Azure/azure-webjobs-sdk/wiki/Queues#set-values-for-webjobs-sdk-constructor-parameters-in-code.</remarks>
    public class AzureQueueNameResolver : INameResolver
    {

        #region Private Members

        private AzureQueueOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AzureQueueNameResolver"/>
        /// </summary>
        /// <param name="options">The </param>
        public AzureQueueNameResolver(AzureQueueOptions options)
        {
            _options = options;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Resolve(string name)
        {
            //RWM: This logic will have to be changed if we want to enable the ability to process more than one set of queues.
            return _options.QueueName;
        }

        #endregion

    }

}