using CloudNimble.BlazorEssentials.IndexedDb;
using CloudNimble.SimpleMessageBus.IndexedDb.Core;
using CloudNimble.SimpleMessageBus.Publish;
using CloudNimble.SimpleMessageBus.Publish.IndexedDb;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{

    /// <summary>
    /// 
    /// </summary>
    public static class SimpleMessageBus_Publish_IndexedDb_WebAssemblyHostBuilderExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configSectionName"></param>
        /// <param name="indexedDbOptions"></param>
        public static void UseIndexedDbMessagePublisher(this WebAssemblyHostBuilder builder, string configSectionName = "SimpleMessageBus:IndexedDb",
            Action<IndexedDbOptions> indexedDbOptions = null)
        {
            ArgumentNullException.ThrowIfNull(builder);

            var config = builder.Configuration.GetSection(configSectionName);
            if (config is null && indexedDbOptions is null)
            {
                builder.Services.AddOptions<IndexedDbOptions>();
            }

            if (config is not null)
            {
                builder.Services.Configure<IndexedDbOptions>(config);
            }

            if (indexedDbOptions is not null)
            {
                builder.Services.Configure(indexedDbOptions);
            }

            builder.Services.AddSingleton<IndexedDbDatabase>();
            builder.Services.AddSingleton<IMessagePublisher, IndexedDbMessagePublisher>();
        }

    }

}
