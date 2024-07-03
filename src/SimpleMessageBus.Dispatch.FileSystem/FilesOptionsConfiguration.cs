using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Leverages the DI framework to make the injected <see cref="FileSystemOptions"/> available to the <see cref="FilesOptions"/> to the default values can be set.
    /// </summary>
    /// <remarks>
    /// From https://benjamincollins.com/blog/using-dependency-injection-while-configuring-services/
    /// </remarks>
    internal class FilesOptionsConfiguration : IConfigureOptions<FilesOptions>
    {

        #region Private Members

        private readonly IOptions<FileSystemOptions> _fileSystemOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="fileSystemOptions"></param>
        public FilesOptionsConfiguration(IOptions<FileSystemOptions> fileSystemOptions)
        {
            _fileSystemOptions = fileSystemOptions;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures a given <see cref="FilesOptions"/> object with the passed-in values from <see cref="FileSystemOptions"/>.
        /// </summary>
        /// <param name="options"></param>
        public void Configure(FilesOptions options)
        {
            options.RootPath = _fileSystemOptions.Value.RootFolder;
        }

        #endregion

    }

}