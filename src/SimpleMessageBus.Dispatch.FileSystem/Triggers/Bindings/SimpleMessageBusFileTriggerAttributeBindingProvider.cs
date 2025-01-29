// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{

    /// <summary>
    /// Provides binding for the SimpleMessageBusFileTriggerAttribute.
    /// </summary>
    internal class SimpleMessageBusFileTriggerAttributeBindingProvider : ITriggerBindingProvider
    {

        #region Private Members

        private readonly IOptions<FileSystemOptions> _options;
        private readonly ILogger _logger;
        private readonly ISimpleMessageBusFileProcessorFactory _fileProcessorFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMessageBusFileTriggerAttributeBindingProvider"/> class.
        /// </summary>
        /// <param name="options">The file system options.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="fileProcessorFactory">The file processor factory.</param>
        /// <exception cref="ArgumentNullException">Thrown when options or fileProcessorFactory is null.</exception>
        public SimpleMessageBusFileTriggerAttributeBindingProvider(IOptions<FileSystemOptions> options, ILoggerFactory loggerFactory, ISimpleMessageBusFileProcessorFactory fileProcessorFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = loggerFactory?.CreateLogger(LogCategories.CreateTriggerCategory("File"));
            _fileProcessorFactory = fileProcessorFactory ?? throw new ArgumentNullException(nameof(fileProcessorFactory));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        /// <summary>
        /// Attempts to create a trigger binding.
        /// </summary>
        /// <param name="context">The trigger binding provider context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the trigger binding if successful; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the parameter type is not supported.</exception>
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<SimpleMessageBusFileTriggerAttribute>(inherit: false);
            if (attribute is null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            // next, verify that the type is one of the types we support
            var types = StreamValueBinder.GetSupportedTypes(FileAccess.Read)
                .Union([typeof(FileStream), typeof(FileSystemEventArgs), typeof(FileInfo)]);

            if (!ValueBinder.MatchParameterType(context.Parameter, types))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind FileTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

            return Task.FromResult<ITriggerBinding>(new SimpleMessageBusFileTriggerBinding(_options, parameter, _logger, _fileProcessorFactory));
        }

        #endregion

    }

}
