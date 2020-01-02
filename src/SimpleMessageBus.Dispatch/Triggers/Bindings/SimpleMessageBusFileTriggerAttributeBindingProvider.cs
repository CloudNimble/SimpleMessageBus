// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{
    internal class SimpleMessageBusFileTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly IOptions<FileSystemOptions> _options;
        private readonly ILogger _logger;
        private readonly ISimpleMessageBusFileProcessorFactory _fileProcessorFactory;

        public SimpleMessageBusFileTriggerAttributeBindingProvider(IOptions<FileSystemOptions> options, ILoggerFactory loggerFactory, ISimpleMessageBusFileProcessorFactory fileProcessorFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = loggerFactory?.CreateLogger(LogCategories.CreateTriggerCategory("File"));
            _fileProcessorFactory = fileProcessorFactory ?? throw new ArgumentNullException(nameof(fileProcessorFactory));
        }

        /// <inheritdoc/>
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<SimpleMessageBusFileTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            // next, verify that the type is one of the types we support
            IEnumerable<Type> types = StreamValueBinder.GetSupportedTypes(FileAccess.Read)
                .Union(new Type[] { typeof(FileStream), typeof(FileSystemEventArgs), typeof(FileInfo) });
            if (!ValueBinder.MatchParameterType(context.Parameter, types))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind FileTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

            return Task.FromResult<ITriggerBinding>(new SimpleMessageBusFileTriggerBinding(_options, parameter, _logger, _fileProcessorFactory));
        }
    }
}
