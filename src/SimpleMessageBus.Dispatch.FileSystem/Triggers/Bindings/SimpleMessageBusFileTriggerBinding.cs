// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{
    internal class SimpleMessageBusFileTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly SimpleMessageBusFileTriggerAttribute _attribute;
        private readonly IOptions<FileSystemOptions> _options;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;
        private readonly BindingDataProvider _bindingDataProvider;
        private readonly ISimpleMessageBusFileProcessorFactory _fileProcessorFactory;
        private readonly ILogger _logger;

        public SimpleMessageBusFileTriggerBinding(IOptions<FileSystemOptions> options, ParameterInfo parameter, ILogger logger, ISimpleMessageBusFileProcessorFactory fileProcessorFactory)
        {
            _options = options;
            _parameter = parameter;
            _logger = logger;
            _fileProcessorFactory = fileProcessorFactory;
            _attribute = parameter.GetCustomAttribute<SimpleMessageBusFileTriggerAttribute>(inherit: false);
            _bindingDataProvider = BindingDataProvider.FromTemplate(_attribute.Path);
            _bindingContract = CreateBindingContract();
        }

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get
            {
                return _bindingContract;
            }
        }

        public Type TriggerValueType
        {
            get { return typeof(FileSystemEventArgs); }
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            if (!(value is FileSystemEventArgs fileEvent))
            {
                string filePath = value as string;
                fileEvent = GetFileArgsFromString(filePath);
            }

            var bindingData = GetBindingData(fileEvent);

            return Task.FromResult<ITriggerData>(new TriggerData(null, bindingData));
        }

        internal static FileSystemEventArgs GetFileArgsFromString(string filePath)
        {            
            if (!string.IsNullOrEmpty(filePath))
            {
                // TODO: This only supports Created events. For Dashboard invocation, how can we
                // handle Change events?
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                return new FileSystemEventArgs(WatcherChangeTypes.Created, directory, fileName);
            }

            return null;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return Task.FromResult<IListener>(new SimpleMessageBusFileListener(_options, _attribute, context.Executor, _logger, _fileProcessorFactory));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            // These path values are validated later during startup.
            string triggerPath = Path.Combine(_options.Value.RootFolder ?? string.Empty, _attribute.Path ?? string.Empty);

            return new SimpleMessageBusFileTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "Enter a file path",
                    Description = string.Format("File event occurred on path '{0}'", _attribute.GetRootPath()),
                    DefaultValue = triggerPath
                }
            };
        }

        private IReadOnlyDictionary<string, Type> CreateBindingContract()
        {
            var contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "FileTrigger", typeof(FileSystemEventArgs) }
            };

            if (_bindingDataProvider.Contract is not null)
            {
                foreach (var item in _bindingDataProvider.Contract)
                {
                    // In case of conflict, binding data from the value type overrides the built-in binding data above.
                    contract[item.Key] = item.Value;
                }
            }

            return contract;
        }

        private IReadOnlyDictionary<string, object> GetBindingData(FileSystemEventArgs value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // built in binding data
            var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "FileTrigger", value }
            };

            string pathRoot = Path.GetDirectoryName(_attribute.Path);
            int idx = value.FullPath.IndexOf(pathRoot, StringComparison.OrdinalIgnoreCase);
            string pathToMatch = value.FullPath.Substring(idx);

            // binding data from the path template
            var bindingDataFromPath = _bindingDataProvider.GetBindingData(pathToMatch);
            if (bindingDataFromPath is not null)
            {
                foreach (var item in bindingDataFromPath)
                {
                    // In case of conflict, binding data from the path overrides
                    // the built-in binding data above.
                    bindingData[item.Key] = item.Value;
                }
            }

            return bindingData;
        }

        private class SimpleMessageBusFileTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                if (arguments is not null && arguments.TryGetValue(Name, out var fullPath))
                {
                    return string.Format("File change detected for file '{0}'", fullPath);
                }
                return null;
            }
        }
    }
}
