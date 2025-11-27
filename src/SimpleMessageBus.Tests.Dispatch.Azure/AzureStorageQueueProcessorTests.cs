using CloudNimble.Breakdance.Azurite;
using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMessageBus.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleMessageBus.Tests.Dispatch.Azure
{

    /// <summary>
    /// Tests for AzureStorageQueueProcessor using Azurite emulator.
    /// </summary>
    [TestClass]
    public partial class AzureStorageQueueProcessorTests : AzuriteBreakdanceTestBase
    {

        public TestContext TestContext { get; set; }

        #region Azurite Setup

        private AzuriteInstance _azurite;

        protected override AzuriteInstance Azurite => _azurite;

        public static int MessageCount = 0;

        [TestInitialize]
        public async Task TestInit()
        {
            _azurite = await CreateAndStartInstanceAsync(new AzuriteConfiguration
            {
                Services = AzuriteServiceType.Queue,
                InMemoryPersistence = true,
                Silent = true
            });
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            try
            {
                // RWM: Disposing the class already stops the host. 
                if (_azurite is not null)
                {
                    await StopAndDisposeAsync(_azurite);
                    _azurite = null;
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine(ex.Message);
            }
        }

        #endregion

        public async Task TestSetup(AzureStorageQueueEncoding encoding)
        {
            // Get the connection string from Azurite
            var azuriteConnectionString = ConnectionString;

            TestHostBuilder
                //.UseEnvironment("Development")
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // Configure AzureWebJobsStorage for WebJobs SDK
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["AzureWebJobsStorage"] = azuriteConnectionString,
                        ["AzureStorageQueueOptions:StorageConnectionString"] = azuriteConnectionString
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessageHandler, TestMessageHandler>();
                })
                .UseAzureStorageQueueMessagePublisher(options =>
                {
                    options.StorageConnectionString = azuriteConnectionString;
                    options.MessageEncoding = encoding;
                })
                .UseAzureStorageQueueProcessor(options =>
                {
                    options.StorageConnectionString = azuriteConnectionString;
                    options.ConcurrentJobs = 1;
                    options.MessageEncoding = encoding;
                })
                .UseOrderedMessageDispatcher()
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();
                })
                .UseConsoleLifetime();

            TestSetup();
            _ = Task.Run(() => TestHost.RunAsync(TestContext.CancellationToken), TestContext.CancellationToken);
        }


        /// <summary>
        ///
        /// </summary>
        [TestMethod]
        public async Task MessagePublisher_NoEncoding_WorksAsDesigned()
        {
            await TestSetup(AzureStorageQueueEncoding.None);

            var publisher = TestHost.Services.GetRequiredService<IMessagePublisher>();

            await publisher.PublishAsync(new TestMessage());
            MessageCount.Should().Be(0);
            await Task.Delay(4000, TestContext.CancellationToken);
            MessageCount.Should().Be(1);
            MessageCount = 0;
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod]
        public async Task MessagePublisher_Base64Encoding_WorksAsDesigned()
        {
            await TestSetup(AzureStorageQueueEncoding.Base64);

            var publisher = TestHost.Services.GetRequiredService<IMessagePublisher>();

            await publisher.PublishAsync(new TestMessage());
            MessageCount.Should().Be(0);
            await Task.Delay(4000, TestContext.CancellationToken);
            MessageCount.Should().Be(1);
            MessageCount = 0;
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
