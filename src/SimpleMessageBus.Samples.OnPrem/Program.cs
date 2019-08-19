using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleMessageBus.Samples.OnPrem
{

    /// <summary>
    /// 
    /// </summary>
    public class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            var rootFolder = @"D:\Scratch\SimpleMessageBus";

            var builder = new HostBuilder()
            .UseEnvironment("Development")

            // RWM: Configure the services before you call Use____QueueProcessor so that the assembly is loaded into memory before the Reflection happens.
            .ConfigureServices(services =>
            {
                services.AddTimerDependencies();
                services.AddSingleton<IMessageHandler, EmailMessageHandler>();
            })
            .UseFileSystemMessagePublisher(options =>
            {
                options.RootFolder = rootFolder;
            })
            .UseFileSystemQueueProcessor(options =>
            {
                options.RootFolder = rootFolder;
            })
            .UseOrderedMessageDispatcher()
            .ConfigureLogging((context, b) =>
            {
                b.SetMinimumLevel(LogLevel.Debug);
                b.AddConsole();
            });

#if netcore3_0
            builder.UseSimpleMessageBusLifetime();
#else
            builder.UseConsoleLifetime();
#endif

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }

    }

}