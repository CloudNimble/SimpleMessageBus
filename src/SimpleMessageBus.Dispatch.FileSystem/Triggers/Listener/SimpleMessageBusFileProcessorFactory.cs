// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{
    internal class SimpleMessageBusFileProcessorFactory : ISimpleMessageBusFileProcessorFactory
    {
        public SimpleMessageBusFileProcessor CreateFileProcessor(SimpleMessageBusFileProcessorFactoryContext context)
        {
            return new SimpleMessageBusFileProcessor(context);
        }
    }
}
