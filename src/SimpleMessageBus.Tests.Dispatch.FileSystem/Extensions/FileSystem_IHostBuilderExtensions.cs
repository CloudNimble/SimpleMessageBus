using CloudNimble.Breakdance.Extensions.MSTest2;
using CloudNimble.SimpleMessageBus.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleMessageBus.Tests.Dispatch.FileSystem.Extensions
{

    [TestClass]
    public class FileSystem_IHostBuilderExtensions : BreakdanceMSTestBase
    {

        [TestInitialize]
        public void Setup()
        {
            TestHostBuilder
                .UseEnvironment("Development")
                .UseFileSystemQueueProcessor()
                .UseOrderedMessageDispatcher();
            TestSetup();
        }

        [TestMethod]
        public void Configuration_CanFindFileSystemOptions()
        {
            var fileSystemOptions = TestHost.Services.GetService<IOptions<FileSystemOptions>>().Value;
            fileSystemOptions.Should().NotBeNull();
            fileSystemOptions.RootFolder.Should().Be(@"C:\Temp\SimpleMessageBus");
            fileSystemOptions.VirusScanDelayInSeconds.Should().Be(0);
        }


    }

}
