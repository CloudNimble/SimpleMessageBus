// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{
    [Extension("SimpleMessageBusFiles")]
    internal class SimpleMessageBusFilesExtensionConfigProvider : IExtensionConfigProvider, IConverter<SimpleMessageBusFileAttribute, Stream>
    {
        private readonly IOptions<FilesOptions> _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISimpleMessageBusFileProcessorFactory _fileProcessorFactory;

        public SimpleMessageBusFilesExtensionConfigProvider(IOptions<FilesOptions> options, ILoggerFactory loggerFactory, ISimpleMessageBusFileProcessorFactory fileProcessorFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
            _fileProcessorFactory = fileProcessorFactory;
        }

        private FileInfo GetFileInfo(SimpleMessageBusFileAttribute attribute)
        {
            var boundFileName = attribute.Path;
            var filePath = Path.Combine(_options.Value.RootPath, boundFileName);
            var fileInfo = new FileInfo(filePath);
            return fileInfo;
        }

        public FileStream GetFileStream(SimpleMessageBusFileAttribute attribute)
        {
            var fileInfo = GetFileInfo(attribute);
            if ((attribute.Access == FileAccess.Read) && !fileInfo.Exists)
            {
                return null;
            }

            return fileInfo.Open(attribute.Mode, attribute.Access);
        }

        public Stream Convert(SimpleMessageBusFileAttribute attribute)
        {
            return GetFileStream(attribute);
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var rule = context.AddBindingRule<SimpleMessageBusFileAttribute>();
            rule.BindToInput(GetFileInfo);
            rule.BindToInput(GetFileStream);
            rule.BindToStream(this, FileAccess.ReadWrite);

            // Triggers
            var rule2 = context.AddBindingRule<SimpleMessageBusFileTriggerAttribute>();
            rule2.BindToTrigger<FileSystemEventArgs>(new SimpleMessageBusFileTriggerAttributeBindingProvider(_options, _loggerFactory, _fileProcessorFactory));

            rule2.AddConverter<string, FileSystemEventArgs>(str => SimpleMessageBusFileTriggerBinding.GetFileArgsFromString(str));
            rule2.AddConverter<FileSystemEventArgs, Stream>(args => File.OpenRead(args.FullPath));
        }
    }
}
