using CloudNimble.BlazorEssentials.IndexedDb;
using CloudNimble.SimpleMessageBus.IndexedDb.Core;
using CloudNimble.SimpleMessageBus.Publish;
using CloudNimble.SimpleMessageBus.Publish.IndexedDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// 
    /// </summary>
    public static class SimpleMessageBus_Publish_IndexedDb_IHostBuilderExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="configSectionName">
        /// The name of the <see cref="ConfigurationSection" /> to load the <see cref="IndexedDbOptions" /> from. Defaults to 'SimpleMessageBus:IndexedDb'.
        /// </param>
        /// <param name="indexedDbOptions">
        /// An <see cref="Action{T}" /> lambda that allows you to set the <see cref="IndexedDbOptions" /> inline.
        /// </param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent configuration.</returns>
        /// 
        public static IHostBuilder UseIndexedDbMessagePublisher(this IHostBuilder builder, string configSectionName = "SimpleMessageBus:IndexedDb", 
            Action<IndexedDbOptions> indexedDbOptions = null)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ConfigureServices((hostContext, services) =>
            {
                var config = hostContext.Configuration.GetSection(configSectionName);
                if (config is null && indexedDbOptions is null)
                {
                    services.AddOptions<IndexedDbOptions>();
                }

                if (config is not null)
                {
                    services.Configure<IndexedDbOptions>(config);
                }

                if (indexedDbOptions is not null)
                {
                    services.Configure(indexedDbOptions);
                }

                services.AddScoped<IndexedDbDatabase>();
                services.AddScoped<IMessagePublisher, IndexedDbMessagePublisher>();
            });

            return builder;
        }

    }

}