using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting.WindowsServices;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus with a DI container.
    /// </summary>
    public static class SimpleMessageBus_Hosting_IHostBuilderExtensions
    {

        /// <summary>
        /// Configures SimpleMessageBus to use the either the <see cref="WindowsServiceLifetime"/> or the <see cref="ConsoleLifetime"/> depending on the currently-running context.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        public static IHostBuilder UseSimpleMessageBusLifetime(this IHostBuilder builder)
        {
            if (WindowsServiceHelpers.IsWindowsService())
            {
                builder.UseWindowsService();
            }
            else
            {
                builder.UseConsoleLifetime();
            }
            return builder;
        }

    }

}