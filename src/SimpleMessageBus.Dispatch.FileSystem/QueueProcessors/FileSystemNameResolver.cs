﻿using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Helps dynamically resolve the name of the configured Queue at runtime.
    /// </summary>
    /// <remarks>See https://github.com/Azure/azure-webjobs-sdk/wiki/Queues#set-values-for-webjobs-sdk-constructor-parameters-in-code.</remarks>
    internal class FileSystemNameResolver : INameResolver
    {

        #region Private Members

        private FileSystemOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="FileSystemNameResolver"/>
        /// </summary>
        /// <param name="options">The <see cref="IOptions{FileSystemOptions}"/> instance injected from the DI container.</param>
        public FileSystemNameResolver(IOptions<FileSystemOptions> options)
        {
            _options = options.Value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Resolve(string name)
        {
            return name switch
            {
                FileSystemConstants.Completed => _options.CompletedFolderPath,
                FileSystemConstants.Error => _options.ErrorFolderPath,
                _ => _options.QueueFolderPath,
            };
        }

        #endregion

    }

}