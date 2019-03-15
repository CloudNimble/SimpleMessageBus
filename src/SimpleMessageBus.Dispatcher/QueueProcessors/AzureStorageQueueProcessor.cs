using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
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

        private readonly AzureQueueOptions _options;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dispatcher"></param>
        public AzureStorageQueueProcessor(AzureQueueOptions options, IMessageDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Please register a FileSystemOptions instance with your DI container.");
            }
            if (string.IsNullOrWhiteSpace(options.QueueName))
            {
                throw new ArgumentNullException(nameof(options.QueueName), "Please specify the path to the folder that will store queue items.");
            }
 
            _options = options;
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;

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
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task ProcessQueue([QueueTrigger("%queue%")] string messageEnvelopeJson, int dequeueCount, TextWriter log)
        {
            using (var lifetimeScope = _serviceProvider.CreateScope())
            {
                var message = JsonConvert.DeserializeObject<MessageEnvelope>(messageEnvelopeJson);
                message.AttemptsCount = dequeueCount;
                message.ProcessLog = log;
                await _dispatcher.Dispatch(message);
            }
        }

        #endregion

        #region Private Methods



        #endregion

    }

}