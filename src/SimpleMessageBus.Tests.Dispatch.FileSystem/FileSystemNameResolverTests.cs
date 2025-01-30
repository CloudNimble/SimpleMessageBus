using CloudNimble.Breakdance.Extensions.MSTest2;
using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.Dispatch;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SimpleMessageBus.Tests
{

    [TestClass]
    public class FileSystemNameResolverTests : BreakdanceMSTestBase
    {

        private string rootPath = Path.Combine(Path.GetTempPath(), "smb-tests");

        #region Test Lifecycle

        /// <summary>
        /// Sets up services needed for tests.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            TestHostBuilder.UseFileSystemQueueProcessor(options => options.RootFolder = rootPath);
            TestSetup();
        }

        #endregion

        [TestMethod]
        public void Resolver_Should_ResolveQueueFolder()
        {
            var resolver = TestHost.Services.GetRequiredService<INameResolver>();
            resolver.GetType().Should().Be<FileSystemNameResolver>();

            resolver.Resolve("queue").Should().Be(Path.Combine(rootPath, FileSystemConstants.Queue));
        }

        [TestMethod]
        public void Resolver_Should_ResolveCompletedFolder()
        {
            var resolver = TestHost.Services.GetRequiredService<INameResolver>();
            resolver.GetType().Should().Be<FileSystemNameResolver>();

            resolver.Resolve("completed").Should().Be(Path.Combine(rootPath, FileSystemConstants.Completed));
        }

        [TestMethod]
        public void Resolver_Should_ResolveErrorFolder()
        {
            var resolver = TestHost.Services.GetRequiredService<INameResolver>();
            resolver.GetType().Should().Be<FileSystemNameResolver>();

            resolver.Resolve("error").Should().Be(Path.Combine(rootPath, FileSystemConstants.Error));
        }

        [TestMethod]
        public void Resolver_Should_ResolveRandomFolder()
        {
            var resolver = TestHost.Services.GetRequiredService<INameResolver>();
            resolver.GetType().Should().Be<FileSystemNameResolver>();

            resolver.Resolve("psychopath").Should().Be(Path.Combine(rootPath, FileSystemConstants.Queue));
        }

    }

}
