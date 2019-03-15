using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// 
    /// </summary>
    public static class IHostBuilderExtensions
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder UseAzureStorageQueueProcessor(this IHostBuilder builder)
        {
            return builder.UseAzureStorageQueueProcessor(o => { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="azureQueueOptions"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder UseFileSystemQueueProcessor(this IHostBuilder builder)
        {
            return builder.UseFileSystemQueueProcessor(o => { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="azureQueueOptions"></param>
        /// <returns></returns>
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

    }

}