// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{

    /// <summary>
    /// Factory interface for creating <see cref="SimpleMessageBusFileProcessor"/> instances. This factory pattern allows
    /// different FileProcessors to be used for different job functions.
    /// </summary>
    public interface ISimpleMessageBusFileProcessorFactory
    {

        /// <summary>
        /// Create a <see cref="SimpleMessageBusFileProcessor"/> for the specified inputs.
        /// </summary>
        /// <param name="context">The context to use.</param>
        /// <returns>The <see cref="SimpleMessageBusFileProcessor"/></returns>
        SimpleMessageBusFileProcessor CreateFileProcessor(SimpleMessageBusFileProcessorFactoryContext context);

    }

}
