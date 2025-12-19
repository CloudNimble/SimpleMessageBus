using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using CloudNimble.SimpleMessageBus.Publish.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus Kafka publishing with a DI container.
    /// </summary>
    public static class SimpleMessageBus_Publish_Kafka_IHostBuilderExtensions
    {

        #region Public Methods

        /// <summary>
        /// Configures SimpleMessageBus to publish messages to Apache Kafka.
        /// Reads configuration from the "KafkaOptions" section of IConfiguration.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
        /// <example>
        /// <code>
        /// Host.CreateDefaultBuilder()
        ///     .UseKafkaMessagePublisher()
        ///     .Build()
        ///     .Run();
        /// </code>
        /// </example>
        public static IHostBuilder UseKafkaMessagePublisher(this IHostBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<KafkaOptions>(hostContext.Configuration.GetSection(typeof(KafkaOptions).Name));
                })
                .UseKafkaMessagePublisher(o => { });
        }

        /// <summary>
        /// Configures SimpleMessageBus to publish messages to Apache Kafka.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <param name="kafkaOptions">An <see cref="Action{KafkaOptions}"/> that provides a fluent interface for configuring Kafka options.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="kafkaOptions"/> is null.</exception>
        /// <example>
        /// <code>
        /// Host.CreateDefaultBuilder()
        ///     .UseKafkaMessagePublisher(options =>
        ///     {
        ///         options.BrokerList = "localhost:9092";
        ///         options.TopicName = "my-events";
        ///     })
        ///     .Build()
        ///     .Run();
        /// </code>
        /// </example>
        public static IHostBuilder UseKafkaMessagePublisher(this IHostBuilder builder, Action<KafkaOptions> kafkaOptions)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(kafkaOptions);

            builder.ConfigureServices((hostContext, services) =>
            {
                services.Configure(kafkaOptions);
                services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();
            });

            return builder;
        }

        #endregion

    }

}
