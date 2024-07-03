using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus with a DI container.
    /// </summary>
    public static class IHostBuilderExtensions
    {

        #region Public Methods

        /// <summary>
        /// Configures SimpleMessageBus to use the <see cref="OrderedMessageDispatcher"/>, which processes registered <see cref="IMessageHandler">IMessageHandlers</see> in series based on
        /// the order they were registered in the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseOrderedMessageDispatcher(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IMessageDispatcher, OrderedMessageDispatcher>();
            });
            return builder;
        }

        /// <summary>
        /// Configures SimpleMessageBus to use the <see cref="ParallelMessageDispatcher"/>, which processes registered <see cref="IMessageHandler">IMessageHandlers</see> in parallel
        /// regardless of the order the order they were registered in the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseParallelMessageDispatcher(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IMessageDispatcher, ParallelMessageDispatcher>();
            });
            return builder;
        }

        #endregion

    }

}