using CloudNimble.Breakdance.Assemblies;
using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Publish;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleMessageBus.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMessageBus.Tests.Dispatch
{

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public partial class AzureStorageQueueProcessorTests : BreakdanceTestBase
    {

        public static int MessageCount = 0;

        public void TestSetup(AzureStorageQueueEncoding encoding)
        {
            TestHostBuilder
                //.UseEnvironment("Development")
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessageHandler, TestMessageHandler>();
                })
                .UseAzureStorageQueueMessagePublisher()
                .UseAzureStorageQueueProcessor(options => { options.ConcurrentJobs = 1; options.MessageEncoding = encoding; })
                .UseOrderedMessageDispatcher()

                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();
                })
                .UseConsoleLifetime();
            TestSetup();
            _ = TestHost.RunAsync();
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task MessagePublisher_NoEncoding_WorksAsDesigned()
        {
            TestSetup(AzureStorageQueueEncoding.None);

            var publisher = TestHost.Services.GetRequiredService<IMessagePublisher>();

            await publisher.PublishAsync(new TestMessage());
            MessageCount.Should().Be(0);
            Thread.Sleep(3000);
            MessageCount.Should().Be(1);
            MessageCount = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task MessagePublisher_Base64Encoding_WorksAsDesigned()
        {
            TestSetup(AzureStorageQueueEncoding.Base64);

            var publisher = TestHost.Services.GetRequiredService<IMessagePublisher>();

            await publisher.PublishAsync(new TestMessage());
            MessageCount.Should().Be(0);
            Thread.Sleep(3000);
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
            public Task OnNextAsync(MessageEnvelope messageEnvelope)
            {
                MessageCount = 1;
                return Task.FromResult(0);
            }

        }

    }

}
