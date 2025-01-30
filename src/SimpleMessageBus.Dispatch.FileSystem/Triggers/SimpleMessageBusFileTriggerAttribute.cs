// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using IOPath = System.IO.Path;
using Microsoft.Azure.WebJobs.Description;
using System.IO;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{

    /// <summary>
    /// Attribute used to mark a job function that should be invoked based
    /// on file events.
    /// </summary>
    /// <remarks>
    /// The method parameter type can be one of the following:
    /// <list type="bullet">
    /// <item><description><see cref="FileStream"/></description></item>
    /// <item><description><see cref="FileInfo"/></description></item>
    /// <item><description><see cref="FileSystemEventArgs"/></description></item>
    /// <item><description><see cref="string"/></description></item>
    /// <item><description><see cref="System.IO.Stream"/></description></item>
    /// <item><description><see cref="System.IO.TextReader"/></description></item>
    /// <item><description><see cref="System.IO.StreamReader"/></description></item>
    /// <item><description>A user-defined type (serialized as JSON)</description></item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class SimpleMessageBusFileTriggerAttribute : Attribute
    {

        #region Fields

        private string _rootPath;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the root path that this trigger is configured to watch for files on.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the root path that this trigger is configured to watch for files on.
        /// </summary>
        public string RootPath => _rootPath;

        /// <summary>
        /// Gets the optional file filter that will be used.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Gets the <see cref="WatcherChangeTypes"/> that will be used by the file watcher.
        /// </summary>
        public WatcherChangeTypes ChangeTypes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether files should be automatically deleted after they
        /// are successfully processed. When set to true, all files including any companion files
        /// starting with the target file name will be deleted when the file is successfully processed.
        /// </summary>
        public bool AutoDelete { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="path">The root path that this trigger is configured to watch for files on.</param>
        /// <param name="filter">The optional file filter that will be used.</param>
        /// <param name="changeTypes">The <see cref="WatcherChangeTypes"/> that will be used by the file watcher. The Default is Created.</param>
        /// <param name="autoDelete">True if processed files should be deleted automatically, false otherwise. The default is False.</param>
        public SimpleMessageBusFileTriggerAttribute(string path, string filter, WatcherChangeTypes changeTypes = WatcherChangeTypes.Created, bool autoDelete = false)
        {
            Path = string.IsNullOrEmpty(path) ? path : path.Replace('/', IOPath.DirectorySeparatorChar);
            Filter = filter;
            ChangeTypes = changeTypes;
            AutoDelete = autoDelete;

            // RWM: Get the root path once instead of recalculating it a bunch of times
            var trimmedPath = Path.TrimEnd(IOPath.DirectorySeparatorChar);
            var index = trimmedPath.LastIndexOf(IOPath.DirectorySeparatorChar);
            if (index > 0 && trimmedPath.IndexOfAny(['{', '}'], index) > 0)
            {
                _rootPath = IOPath.GetDirectoryName(trimmedPath);
            }
            _rootPath = Path;
        }

        #endregion

    }

}
