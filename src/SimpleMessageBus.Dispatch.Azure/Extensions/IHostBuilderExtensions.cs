using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus with a DI container.
    /// </summary>
    public static class IHostBuilderExtensions
    {

        #region Public Methods

        /// <summary>
        /// Configures SimpleMessageBus to use Azure Storage Queues as the backing queue and registers the <see cref="AzureStorageQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureStorageQueueProcessor(this IHostBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .ConfigureServices((hostContext, services) => 
                 {
                     services.Configure<AzureStorageQueueOptions>(hostContext.Configuration.GetSection(typeof(AzureStorageQueueOptions).Name));
                 })
                 .UseAzureStorageQueueProcessor(o => { });
        }

        /// <summary>
        /// Configures SimpleMessageBus to use Azure Storage Queues as the backing queue and registers the <see cref="AzureStorageQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="azureQueueOptions">An <see cref="Action{AzureQueueOptions}"/> that gives you a fluent interface for configuring the options for a queue backed by Azure Queue Storage.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureStorageQueueProcessor(this IHostBuilder builder, Action<AzureStorageQueueOptions> azureQueueOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (azureQueueOptions is null)
            {
                throw new ArgumentNullException(nameof(azureQueueOptions));
            }

            builder.ConfigureWebJobs(config =>
            {
                config.AddAzureStorageCoreServices();
                config.AddBuiltInBindings();
                config.AddAzureStorageQueues();
                config.AddTimers();
            })

            .ConfigureServices((hostContext, services) =>
            {
                //RWM: This is a total hack, but I can't figure out why there are more than 1 in here, so we're hacking for now.
                //hostContext.FixWebJobsRegistration();

                services.Configure(azureQueueOptions);
                services.AddSingleton<IConfigureOptions<QueuesOptions>, QueuesOptionsConfiguration>();
                services.AddSingleton<INameResolver, AzureStorageQueueNameResolver>();
                services.AddSingleton<IQueueProcessor, AzureStorageQueueProcessor>();
            });

            return builder;
        }

        #endregion

    }

}