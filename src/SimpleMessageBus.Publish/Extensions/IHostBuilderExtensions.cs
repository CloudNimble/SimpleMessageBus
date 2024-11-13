using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// 
    /// </summary>
    public static class SimpleMessageBus_Publish_IHostBuilderExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemMessagePublisher(this IHostBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<FileSystemOptions>(hostContext.Configuration.GetSection(typeof(FileSystemOptions).Name));
                })
                .UseFileSystemMessagePublisher(o => { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="fileSystemOptions"></param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemMessagePublisher(this IHostBuilder builder, Action<FileSystemOptions> fileSystemOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (fileSystemOptions is null)
            {
                throw new ArgumentNullException(nameof(fileSystemOptions));
            }

            builder.ConfigureServices((hostContext, services) =>
            {
                services.Configure(fileSystemOptions);
                services.AddSingleton<IMessagePublisher, FileSystemMessagePublisher>();
            });

            return builder;
        }

    }

}