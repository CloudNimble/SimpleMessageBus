using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
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
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureQueueMessagePublisher(this IHostBuilder builder)
        {
            return builder.UseAzureQueueMessagePublisher(o => { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="azureQueueOptions"></param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureQueueMessagePublisher(this IHostBuilder builder, Action<AzureQueueOptions> azureQueueOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (azureQueueOptions == null)
            {
                throw new ArgumentNullException(nameof(azureQueueOptions));
            }

            builder.ConfigureServices(services =>
            {
                services.Configure(azureQueueOptions);
                services.AddSingleton<IMessagePublisher, AzureQueueMessagePublisher>();
            });

            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemMessagePublisher(this IHostBuilder builder)
        {
            return builder.UseFileSystemMessagePublisher(o => { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="fileSystemOptions"></param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemMessagePublisher(this IHostBuilder builder, Action<FileSystemOptions> fileSystemOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (fileSystemOptions == null)
            {
                throw new ArgumentNullException(nameof(fileSystemOptions));
            }

            builder.ConfigureServices(services =>
            {
                services.Configure(fileSystemOptions);
                services.AddSingleton<IMessagePublisher, FileSystemMessagePublisher>();
            });

            return builder;
        }

    }

}