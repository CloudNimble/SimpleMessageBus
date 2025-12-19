using CloudNimble.Breakdance.Assemblies;
using CloudNimble.Breakdance.Extensions.MSTest2;
using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SimpleMessageBus.Tests.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMessageBus.Tests.Dispatch
{

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class FileSystemQueueProcessorTests : BreakdanceMSTestBase
    {

        public static int MessageCount = 0;

        private const string filePath = @"D:\Scratch\SimpleMessageBus\";

        [TestInitialize]
        public async Task TestInitAsync()
        {
            TestHostBuilder
                .UseEnvironment("Development")
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessageHandler, TestMessageHandler>();
                })

                .UseFileSystemMessagePublisher(options =>
                {
                    options.RootFolder = filePath;
                })
                .UseFileSystemQueueProcessor(options =>
                {
                    options.RootFolder = filePath;
                    options.VirusScanDelayInSeconds = 5;
                })
                .UseOrderedMessageDispatcher()

                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();
                })
                .UseConsoleLifetime();

            TestSetup();
            _ = TestHost.RunAsync(TestContext.CancellationToken);
        }

        [TestCleanup]
        public async Task DeleteQueueFiles()
        {
            var options = TestHost.Services.GetRequiredService<IOptions<FileSystemOptions>>().Value;
            foreach (var file in Directory.GetFiles(options.QueueFolderPath))
            {
                File.Delete(file);
            }
            TestTearDown();
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task RegularMessagePublisherWorks()
        {
            var publisher = TestHost.Services.GetRequiredService<IMessagePublisher>();
            await publisher.PublishAsync(new TestMessage());
            Thread.Sleep(3000);
            MessageCount.Should().Be(0);
            Thread.Sleep(3000);
            MessageCount.Should().Be(1);
            MessageCount = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task SimulatedNetworkMessagePublisherWorks()
        {
            var time = DateTime.Now.ToString("HHmmss");

            var envelope = new MessageEnvelope(new TestMessage())
            {
                Id = Guid.NewGuid(),
                DatePublished = DateTimeOffset.UtcNow
            };

            var options = TestHost.Services.GetRequiredService<IOptions<FileSystemOptions>>().Value;

            File.WriteAllText(Path.Combine(options.QueueFolderPath, $"{time}.tmpmsg"), JsonConvert.SerializeObject(envelope));
            Thread.Sleep(200);
            File.Move(Path.Combine(options.QueueFolderPath, $"{time}.tmpmsg"), Path.Combine(options.QueueFolderPath, $"{time}.json"));

            Thread.Sleep(3000);
            MessageCount.Should().Be(0);
            Thread.Sleep(3000);
            MessageCount.Should().Be(1);
            MessageCount = 0;
            await Task.FromResult(0);
        }

        private class TestMessageHandler : IMessageHandler
        {

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Type> GetHandledMessageTypes()
            {
                Console.WriteLine("TestMessageHandler Loaded.");
                yield return typeof(TestMessage);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            /// <param name="exception"></param>
            /// <returns></returns>
            public Task OnErrorAsync(IMessage message, Exception exception) => throw new NotImplementedException();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="messageEnvelope"></param>
            /// <returns></returns>
            public async Task OnNextAsync(MessageEnvelope messageEnvelope)
            {
                MessageCount = 1;
                Console.WriteLine("MessageCount Incremented.");
                await Task.FromResult(true);
            }

        }

    }

}
