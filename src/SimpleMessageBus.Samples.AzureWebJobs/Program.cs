using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleMessageBus.Samples.AzureWebJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder()
                // RWM: Configure the services *before* you call Use____QueueProcessor so that the assembly is loaded into memory before the WebJobs' Reflection happens.
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTimerDependencies();
                    services.AddSingleton<IMessageHandler, EmailMessageHandler>();
                })
                .UseAzureStorageQueueMessagePublisher()
                .UseAzureStorageQueueProcessor(options => options.ConcurrentJobs = 1)
                .UseOrderedMessageDispatcher()
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();
                    string instrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                    if (!string.IsNullOrEmpty(instrumentationKey))
                    {
                        b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = instrumentationKey);
                    }
                });

            builder.UseSimpleMessageBusLifetime();

            var host = builder.Build();
            using (host)
                host.Run();
        }
    }
}
