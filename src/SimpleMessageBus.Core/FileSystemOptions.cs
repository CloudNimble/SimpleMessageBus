using System.IO;

namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Specifies the options required to leverage the local file system as the SimpleMessageBus backing queue.
    /// </summary>
    public class FileSystemOptions
    {

        #region Private members

        private bool? _isNetworkPath;

        #endregion

        #region Properties

        /// <summary>
        /// The folder segment where successfully-processed queue items will be moved to upon completion.
        /// </summary>
        public string CompletedFolderPath => Path.Combine(RootFolder, FileSystemConstants.Completed);

        /// <summary>
        /// The folder segment where failed items will be stored while they are waiting to be analyzed and reprocessed.
        /// </summary>
        public string ErrorFolderPath => Path.Combine(RootFolder, FileSystemConstants.Error);

        /// <summary>
        /// Gets a boolean specifying whether or not the <see cref="RootFolder"/> is a network path (either a UNC or mapped drive).
        /// </summary>
        public bool IsNetworkPath
        {
            get
            {
                if (_isNetworkPath != null) return _isNetworkPath.Value;

                string root = Path.GetPathRoot(RootFolder);

                _isNetworkPath = true switch
                {
                    // Check if root starts with "\\", clearly an UNC
                    true when root.StartsWith(@"\\") => true,
                    // Check if the drive is a network drive
                    true when new DriveInfo(root).DriveType == DriveType.Network => true,
                    _ => false,
                };
                return _isNetworkPath.Value;
            }
        }

        /// <summary>
        /// The folder segment where items will be stored while they are waiting to be processed.
        /// </summary>
        public string QueueFolderPath => Path.Combine(RootFolder, FileSystemConstants.Queue);

        /// <summary>
        /// A string representing the folder that will hold the three required queue folders.
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// An integr representing the number of seconds to wait before firing FileSystemWatcher events to process the Queue.
        /// </summary>
        public int VirusScanDelayInSeconds { get; set; } = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor, which sets the default values equal to the values specified in <see cref="FileSystemConstants"/>.
        /// </summary>
        public FileSystemOptions()
        {
        }

        #endregion

    }

}