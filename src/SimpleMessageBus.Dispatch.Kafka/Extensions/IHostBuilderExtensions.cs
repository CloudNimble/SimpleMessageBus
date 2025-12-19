using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using CloudNimble.SimpleMessageBus.Dispatch.Kafka;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;

using KafkaOptions = CloudNimble.SimpleMessageBus.Core.KafkaOptions;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus Kafka dispatching with a DI container.
    /// </summary>
    public static class SimpleMessageBus_Dispatch_Kafka_IHostBuilderExtensions
    {

        #region Public Methods

        /// <summary>
        /// Configures SimpleMessageBus to process messages from Apache Kafka and registers the <see cref="KafkaProcessor"/> with the DI container.
        /// Reads configuration from the "KafkaOptions" section of IConfiguration.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
        /// <example>
        /// <code>
        /// Host.CreateDefaultBuilder()
        ///     .UseKafkaProcessor()
        ///     .UseOrderedMessageDispatcher()
        ///     .Build()
        ///     .Run();
        /// </code>
        /// </example>
        public static IHostBuilder UseKafkaProcessor(this IHostBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<KafkaOptions>(hostContext.Configuration.GetSection(typeof(KafkaOptions).Name));
                })
                .UseKafkaProcessor(o => { });
        }

        /// <summary>
        /// Configures SimpleMessageBus to process messages from Apache Kafka and registers the <see cref="KafkaProcessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="kafkaOptions">An <see cref="Action{KafkaOptions}"/> that provides a fluent interface for configuring Kafka options.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="kafkaOptions"/> is null.</exception>
        /// <example>
        /// <code>
        /// Host.CreateDefaultBuilder()
        ///     .ConfigureServices(services =>
        ///     {
        ///         services.AddSingleton&lt;IMessageHandler, MyMessageHandler&gt;();
        ///     })
        ///     .UseKafkaProcessor(options =>
        ///     {
        ///         options.BrokerList = "localhost:9092";
        ///         options.ConsumerGroup = "my-consumer";
        ///     })
        ///     .UseOrderedMessageDispatcher()
        ///     .Build()
        ///     .Run();
        /// </code>
        /// </example>
        public static IHostBuilder UseKafkaProcessor(this IHostBuilder builder, Action<KafkaOptions> kafkaOptions)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(kafkaOptions);

            builder
                .ConfigureWebJobs(config =>
                {
                    config.AddBuiltInBindings();
                    config.AddKafka();
                    config.AddTimers();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure(kafkaOptions);
                    services.AddSingleton<INameResolver, KafkaNameResolver>();
                    services.AddSingleton<IQueueProcessor, KafkaProcessor>();
                });

            return builder;
        }

        #endregion

    }

}
