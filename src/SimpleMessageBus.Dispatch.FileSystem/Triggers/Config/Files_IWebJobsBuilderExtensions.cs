﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Files;
using Microsoft.Extensions.DependencyInjection;

namespace CloudNimble.SimpleMessageBus.Dispatch.Triggers
{
    /// <summary>
    /// Extension methods for Files integration
    /// </summary>
    public static class Files_IWebJobsBuilderExtensions
    {
        /// <summary>
        /// Adds the Files extension to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        public static IWebJobsBuilder AddSimpleMessageBusFiles(this IWebJobsBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<SimpleMessageBusFilesExtensionConfigProvider>()
                .BindOptions<FilesOptions>();
            builder.Services.AddSingleton<INameResolver, FileSystemNameResolver>();
            builder.Services.AddSingleton<ISimpleMessageBusFileProcessorFactory, SimpleMessageBusFileProcessorFactory>();

            return builder;
        }

        /// <summary>
        /// Adds the Files extension to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        /// <param name="configure">An <see cref="Action{FilesOptions}"/> to configure the provided <see cref="FilesOptions"/>.</param>
        public static IWebJobsBuilder AddSimpleMessageBusFiles(this IWebJobsBuilder builder, Action<FilesOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddSimpleMessageBusFiles();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
