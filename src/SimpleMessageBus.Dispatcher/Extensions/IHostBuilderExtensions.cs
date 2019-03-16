using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus with a DI container.
    /// </summary>
    public static class IHostBuilderExtensions
    {

        /// <summary>
        /// Configures SimpleMessageBus to use Azure Storage Queues as the backing queue and registers the <see cref="AzureStorageQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureStorageQueueProcessor(this IHostBuilder builder)
        {
            return builder.UseAzureStorageQueueProcessor(o => { });
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
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<INameResolver, AzureQueueNameResolver>();
                services.AddSingleton<IQueueProcessor, AzureStorageQueueProcessor>();
            })
            .UseConsoleLifetime();

            return builder;
        }

        /// <summary>
        /// Configures SimpleMessageBus to use the local file system as the backing queue and registers the <see cref="FileSystemQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemQueueProcessor(this IHostBuilder builder)
        {
            return builder.UseFileSystemQueueProcessor(o => { });
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
                config.AddFiles(options =>
                {
                    //TODO: RWM: Pull this from configuration.
                    options.RootPath = @"D:\Scratch\Queue\";
                });
            })
            .ConfigureServices(services =>
            {
                services.Configure(fileSystemOptions);
                services.AddSingleton<INameResolver, FileSystemNameResolver>(); ;
                services.AddSingleton<IQueueProcessor, FileSystemQueueProcessor>();
            })
            .UseConsoleLifetime();

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

    }

}