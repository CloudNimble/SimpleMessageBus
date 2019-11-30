using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using CloudNimble.SimpleMessageBus.Dispatch.Triggers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .ConfigureServices((hostContext, services) => 
                 {
                     services.Configure<AzureQueueOptions>(hostContext.Configuration.GetSection(typeof(AzureQueueOptions).Name));
                 })
                 .UseAzureStorageQueueProcessor(o => { });
        }

        /// <summary>
        /// Configures SimpleMessageBus to use Azure Storage Queues as the backing queue and registers the <see cref="AzureStorageQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="azureQueueOptions">An <see cref="Action{AzureQueueOptions}"/> that gives you a fluent interface for configuring the options for a queue backed by Azure Queue Storage.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureStorageQueueProcessor(this IHostBuilder builder, Action<AzureQueueOptions> azureQueueOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (azureQueueOptions == null)
            {
                throw new ArgumentNullException(nameof(azureQueueOptions));
            }

            builder.ConfigureWebJobs(config =>
            {
                config.AddAzureStorageCoreServices();
                config.AddAzureStorage();
                config.AddTimers();
            })

            .ConfigureServices((hostContext, services) =>
            {
                //RWM: This is a total hack, but I can't figure out why there are more than 1 in here, so we're hacking for now.
                hostContext.FixWebJobsRegistration();

                services.AddSingleton<INameResolver, AzureQueueNameResolver>();
                services.AddSingleton<IQueueProcessor, AzureStorageQueueProcessor>();
            });

            return builder;
        }

        /// <summary>
        /// Configures SimpleMessageBus to use the local file system as the backing queue and registers the <see cref="FileSystemQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemQueueProcessor(this IHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<FileSystemOptions>(hostContext.Configuration.GetSection(typeof(FileSystemOptions).Name));
                })
                .UseFileSystemQueueProcessor(o => { });
        }

        /// <summary>
        /// Configures SimpleMessageBus to use the local file system as the backing queue and registers the <see cref="FileSystemQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="fileSystemOptions">An <see cref="Action{FileSystemOptions}"/> that gives you a fluent interface for configuring the options for a queue backed by the file system..</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemQueueProcessor(this IHostBuilder builder, Action<FileSystemOptions> fileSystemOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (fileSystemOptions == null)
            {
                throw new ArgumentNullException(nameof(fileSystemOptions));
            }

            builder.ConfigureWebJobs(config =>
            {
                config.AddSimpleMessageBusFiles();
                config.AddTimers();
            })
            .ConfigureServices((hostContext, services) =>
            {
                //RWM: This is a total hack, but I can't figure out why there are more than 1 in here, so we're hacking for now.
                hostContext.FixWebJobsRegistration();

                // RWM: The WebJobs SDK registers the Azure ScheduleMonitor by default. For the local filesystem, we need to rip that out and replace it.
                var oldMonitor = services.First(c => c.ServiceType.Name == nameof(ScheduleMonitor));
                services.Remove(oldMonitor);
                services.AddSingleton<ScheduleMonitor, FileSystemScheduleMonitor>();

                services.Configure(fileSystemOptions);
                services.AddSingleton<IConfigureOptions<FilesOptions>, FilesOptionsConfiguration>();
                services.AddSingleton<INameResolver, FileSystemNameResolver>(); ;
                services.AddSingleton<IQueueProcessor, FileSystemQueueProcessor>();
            });

            return builder;
        }

        /// <summary>
        /// Configures SimpleMessageBus to use the <see cref="OrderedMessageDispatcher"/>, which processes registered <see cref="IMessageHandler">IMessageHandlers</see> in series based on
        /// the order they were registered in the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseOrderedMessageDispatcher(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IMessageDispatcher, OrderedMessageDispatcher>();
            });
            return builder;
        }

        /// <summary>
        /// Configures SimpleMessageBus to use the <see cref="ParallelMessageDispatcher"/>, which processes registered <see cref="IMessageHandler">IMessageHandlers</see> in parallel
        /// regardless of the order the order they were registered in the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseParallelMessageDispatcher(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IMessageDispatcher, ParallelMessageDispatcher>();
            });
            return builder;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The WebJobs SDK (for some reason) calls the Host.AddAppConfiguration for you. The holders for those methods are internal, and the last entry in the pipe overrides previous merging.
        /// So we have to delete the unnecessary ConfigurationProviders until the WebJobs team fixes their mistake.
        /// </summary>
        /// <param name="hostContext"></param>
        private static void FixWebJobsRegistration(this HostBuilderContext hostContext)
        {
            var configProviders = ((hostContext.Configuration as ConfigurationRoot).Providers as List<IConfigurationProvider>);

            if (configProviders.Count(c => c.GetType() == typeof(JsonConfigurationProvider) && (c as JsonConfigurationProvider).Source.Path == "appsettings.json") > 1)
            {
                configProviders.Remove(configProviders.Last(c => c.GetType() == typeof(JsonConfigurationProvider) && (c as JsonConfigurationProvider).Source.Path == "appsettings.json"));
            }

            if (configProviders.Count(c => c.GetType() == typeof(EnvironmentVariablesConfigurationProvider)) > 1)
            {
                configProviders.Remove(configProviders.Last(c => c.GetType() == typeof(EnvironmentVariablesConfigurationProvider)));
            }
        }

        #endregion

    }

}