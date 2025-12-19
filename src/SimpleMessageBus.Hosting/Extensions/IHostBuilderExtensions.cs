using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting.WindowsServices;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// A set of <see cref="IHostBuilder"/> extension methods that make it easy to register SimpleMessageBus with a DI container.
    /// </summary>
    /// <remarks>
    /// These extensions provide hosting utilities for SimpleMessageBus applications, particularly for scenarios
    /// where the application needs to run as either a Windows Service or a console application depending on
    /// the execution context.
    /// </remarks>
    public static class SimpleMessageBus_Hosting_IHostBuilderExtensions
    {

        /// <summary>
        /// Configures SimpleMessageBus to use either the <see cref="WindowsServiceLifetime"/> or the <see cref="ConsoleLifetime"/> depending on the currently-running context.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to extend.</param>
        /// <returns>The <see cref="IHostBuilder"/> instance being configured, for fluent interaction.</returns>
        /// <remarks>
        /// This method automatically detects whether the application is running as a Windows Service
        /// and configures the appropriate lifetime management. This enables the same application
        /// to run seamlessly in both development (console) and production (service) environments.
        /// </remarks>
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