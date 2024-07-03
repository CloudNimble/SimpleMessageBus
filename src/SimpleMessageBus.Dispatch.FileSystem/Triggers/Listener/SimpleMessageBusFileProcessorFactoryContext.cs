// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.Logging;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{
    /// <summary>
    /// Context input for <see cref="ISimpleMessageBusFileProcessorFactory"/>
    /// </summary>
    public class SimpleMessageBusFileProcessorFactoryContext
    {
        /// <summary>
        /// Constructs a new instance
        /// </summary>
        /// <param name="options">The <see cref="FilesOptions"/></param>
        /// <param name="attribute">The <see cref="SimpleMessageBusFileTriggerAttribute"/></param>
        /// <param name="executor">The function executor.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public SimpleMessageBusFileProcessorFactoryContext(FileSystemOptions options, SimpleMessageBusFileTriggerAttribute attribute, ITriggeredFunctionExecutor executor, ILogger logger)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            Executor = executor ?? throw new ArgumentNullException(nameof(executor));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the <see cref="FilesOptions"/>
        /// </summary>
        public FileSystemOptions Options { get; private set; }

        /// <summary>
        /// Gets the <see cref="SimpleMessageBusFileTriggerAttribute"/>
        /// </summary>
        public SimpleMessageBusFileTriggerAttribute Attribute { get; private set; }

        /// <summary>
        /// Gets the function executor
        /// </summary>
        public ITriggeredFunctionExecutor Executor { get; private set; }

        /// <summary>
        /// Gets the <see cref="ILogger"/>.
        /// </summary>
        public ILogger Logger { get; private set; }
    }
}
