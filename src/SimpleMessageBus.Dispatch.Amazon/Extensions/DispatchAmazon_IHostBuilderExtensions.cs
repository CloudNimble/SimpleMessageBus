using CloudNimble.SimpleMessageBus.Dispatch;
using CloudNimble.SimpleMessageBus.Dispatch.Amazon;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus with a DI container.
    /// </summary>
    public static class DispatchAmazon_IHostBuilderExtensions
    {

        #region Public Methods

        /// <summary>
        /// Configures SimpleMessageBus to use Amazon SQS as the backing queue and registers the <see cref="AmazonSQSProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAmazonSQSProcessor(this IHostBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder
                .ConfigureServices((hostContext, services) => 
                 {
                     services.Configure<AmazonSQSOptions>(hostContext.Configuration.GetSection(typeof(AmazonSQSOptions).Name));
                 })
                 .UseAmazonSQSProcessor(o => { });
        }

        /// <summary>
        /// Configures SimpleMessageBus to use Amazon SQS as the backing queue and registers the <see cref="AmazonSQSProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="amazonSQSOptions">An <see cref="Action{AmazonSQSOptions}"/> that gives you a fluent interface for configuring the options for a queue backed by Amazon SQS.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseAmazonSQSProcessor(this IHostBuilder builder, Action<AmazonSQSOptions> amazonSQSOptions)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(amazonSQSOptions);

            builder.ConfigureWebJobs(config =>
            {
                config
                    .AddBuiltInBindings()
                    .AddTimers()
                    .AddAmazonSQS();
            })

            .ConfigureServices((hostContext, services) =>
            {
                services
                    .Configure(amazonSQSOptions)
                    .AddSingleton<INameResolver, AmazonSQSNameResolver>()
                    .AddSingleton<IQueueProcessor, AmazonSQSProcessor>();
            });

            return builder;
        }

        #endregion

    }

}