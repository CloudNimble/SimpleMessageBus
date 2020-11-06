using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

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

#if netcore3_0
            var builder = Host.CreateDefaultBuilder()
#else
            var builder = new HostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();
                })
#endif
                // RWM: Configure the services before you call Use____QueueProcessor so that the assembly is loaded into memory before the Reflection happens.
                .ConfigureServices((hostContext, services) =>
                {
                    Console.WriteLine($"SimpleMessageBus starting in the {hostContext.HostingEnvironment.EnvironmentName} Environment");
                    services.AddTimerDependencies();
                    services.AddSingleton<IMessageHandler, EmailMessageHandler>();
                })
                .UseFileSystemMessagePublisher()
                .UseFileSystemQueueProcessor()
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
            host.Run();
        }

    }

}