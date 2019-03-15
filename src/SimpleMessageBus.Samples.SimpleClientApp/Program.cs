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
            .ConfigureServices(services =>
            {
                services.AddSingleton<IMessagePublisher, FileSystemMessagePublisher>();
            })
            .UseConsoleLifetime();

            _host = builder.Build();
            _host.RunAsync();

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