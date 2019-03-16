using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleMessageBus.Samples.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleMessageBus.Samples.SimpleClientApp
{
    class Program
    {

        private static IHost _host;

        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
            .UseEnvironment("Development")
            .UseFileSystemMessagePublisher(options =>
            {
                options.QueueFolderPath = Path.Combine(@"D:\Scratch\Queue\", FileSystemConstants.Queue);
            })
            .ConfigureLogging((context, b) =>
            {
                b.SetMinimumLevel(LogLevel.Debug);
                b.AddConsole();
            })
            .UseConsoleLifetime();

            _host = builder.Build();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _host.RunAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await CreateMessage(_host.Services.GetRequiredService<IMessagePublisher>());
        }

        public static async Task CreateMessage(IMessagePublisher publisher)
        {
            var message = new NewUserMessage()
            {
                Id = Guid.NewGuid(),
                Email = "robert@nimbleapps.cloud"
            };
            await publisher.PublishAsync(message);
        }

    }

}