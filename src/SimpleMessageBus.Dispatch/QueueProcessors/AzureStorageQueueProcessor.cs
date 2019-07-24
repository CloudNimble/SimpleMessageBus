using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// 
    /// </summary>
    public class AzureStorageQueueProcessor : IQueueProcessor
    {

        #region Private Members

        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="serviceProvider"></param>
        public AzureStorageQueueProcessor(IMessageDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "Please call \".UseOrderedMessageDispatcher()\" or \".UseParallelMessageDispatcher()\" in your Dependency Injection service registration.");
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(dispatcher), "The DependencyInjection IServiceProvider could not be found. Please ensure you've properly registered DI.");

            //// RWM: This needs to refactor to an extension method on IHostBuilder.
            //var builder = new HostBuilder();
            //builder.ConfigureWebJobs(config =>
            //{
            //    config.AddAzureStorageCoreServices();
            //    config.AddAzureStorage(o =>
            //    {
            //        o.MaxDequeueCount = 1;
            //    });
            //});
            //builder.ConfigureServices(s => s.AddSingleton<INameResolver>(new AzureQueueNameResolver(options)));
            //_host = builder.Build();


            //var config = new JobHostConfiguration();
            //config.JobActivator = this;
            //config.DashboardConnectionString = configuration.DashboardConnectionString;
            //config.StorageConnectionString = configuration.StorageConnectionString;
            //config.Queues.BatchSize = configuration.BatchSize; //16
            //config.Queues.MaxDequeueCount = 1; //5
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageEnvelopeJson"></param>
        /// <param name="dequeueCount"></param>
        /// <param name="logger"></param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        public async Task ProcessQueue([QueueTrigger("%queue%")] string messageEnvelopeJson, int dequeueCount, ILogger logger)
        {
            using (var lifetimeScope = _serviceProvider.CreateScope())
            {
                var message = JsonConvert.DeserializeObject<MessageEnvelope>(messageEnvelopeJson);
                message.AttemptsCount = dequeueCount;
                message.ProcessLog = logger;
                await _dispatcher.Dispatch(message).ConfigureAwait(false);
            }
        }

        #endregion

        #region Private Methods



        #endregion

    }

}