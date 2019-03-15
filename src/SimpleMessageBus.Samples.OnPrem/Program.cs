using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
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
            var builder = new HostBuilder()
            .UseEnvironment("Development")
            .UseFileSystemQueueProcessor(options =>
            {
                options.RootFolder = @"D:\Scratch\Queue";
                options.QueueFolderPath = FileSystemConstants.Queue;
                options.CompletedFolderPath = FileSystemConstants.Completed;
                options.ErrorFolderPath = FileSystemConstants.Error;
            })
            .ConfigureLogging((context, b) =>
            {
                b.SetMinimumLevel(LogLevel.Debug);
                b.AddConsole();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<IMessageDispatcher, OrderedMessageDispatcher>();
                services.AddSingleton<IMessageHandler, TestMessageHandler>();
            });

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }

    }

}