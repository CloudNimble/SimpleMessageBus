using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using CloudNimble.SimpleMessageBus.Dispatch.Triggers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
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
        /// Configures SimpleMessageBus to use the local file system as the backing queue and registers the <see cref="FileSystemQueueProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseFileSystemQueueProcessor(this IHostBuilder builder)
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
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (fileSystemOptions is null)
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
                //hostContext.FixWebJobsRegistration();

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

        #endregion

    }

}