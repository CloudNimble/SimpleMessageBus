namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Specifies the options required to leverage the local file system as the SimpleMessageBus backing queue.
    /// </summary>
    public class FileSystemOptions
    {

        #region Properties

        /// <summary>
        /// A string representing the folder that will hold the three required queue folders.
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// The folder segment where successfully-processed queue items will be moved to upon completion.
        /// </summary>
        public string CompletedFolderPath { get; set; }

        /// <summary>
        /// The folder segment where items will be stored while they are waiting to be processed.
        /// </summary>
        public string QueueFolderPath { get; set; }

        /// <summary>
        /// The folder segment where failed items will be stored while they are waiting to be analyzed and reprocessed.
        /// </summary>
        public string ErrorFolderPath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor, which sets the default values equal to the values specified in <see cref="FileSystemConstants"/>.
        /// </summary>
        public FileSystemOptions()
        {
            QueueFolderPath = FileSystemConstants.Queue;
            CompletedFolderPath = FileSystemConstants.Completed;
            ErrorFolderPath = FileSystemConstants.Error;
        }

        #endregion

    }

}