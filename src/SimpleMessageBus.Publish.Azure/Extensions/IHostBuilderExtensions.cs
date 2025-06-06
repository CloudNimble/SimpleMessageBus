﻿using CloudNimble.SimpleMessageBus.Core;
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
        public static IHostBuilder UseAzureStorageQueueMessagePublisher(this IHostBuilder builder)
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
                .UseAzureStorageQueueMessagePublisher(o => { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="azureQueueOptions"></param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAzureStorageQueueMessagePublisher(this IHostBuilder builder, Action<AzureStorageQueueOptions> azureQueueOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (azureQueueOptions is null)
            {
                throw new ArgumentNullException(nameof(azureQueueOptions));
            }

            builder.ConfigureServices((hostContext, services) =>
            {
                services.Configure(azureQueueOptions);
                services.AddSingleton<IMessagePublisher, AzureStorageQueueMessagePublisher>();
            });

            return builder;
        }

    }

}