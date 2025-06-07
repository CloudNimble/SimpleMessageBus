<h1 align="center">SimpleMessageBus - By CloudNimble</h1>
<br>
<h4 align="center">A framework for reliable, distributed, scalable, cross-platform event processing on .NET.</h4>

<div align="center">

<br>

[Website][website-link] &nbsp;&nbsp;&nbsp; | &nbsp;&nbsp;&nbsp; [Releases][release-link] &nbsp;&nbsp;&nbsp;| &nbsp;&nbsp;&nbsp; [Documentation][doc-link] &nbsp;&nbsp;&nbsp;

[![Build Status][devops-rtm-build-img]][devops-rtm-build] [![Release Status][devops-rtm-release-img]][devops-rtm-release] [![Twitter][twitter-img]][twitter-intent]

</div>

## What is SimpleMessageBus?
**SimpleMessageBus** is a system for making applications more reliable and responsive to users by processing potentially long-running tasks out-of-band from
the user's main workflow. It is designed to run either on-prem, or in the Microsoft Cloud, making it suitable for any application, and able to grow as 
your needs do.

### Benefits
 - Allows you to build more user-responsive apps.
 - Increases testability by decoupling long-latency events from UI-generated workflows.
 - Pushes third-party dependencies to the edges of your app, streamlining deployments.
 - Increases fault-tolerance by allowing you to easily track and replay failed messages.


### Ecosystem

#### Core Packages
| Project | Release | Latest | Description |
|---------|--------|-------------|-------------|
| [SimpleMessageBus.Core][smb-core-nuget]    | [![smb-core-rtm][smb-core-rtm-nuget-img]][smb-core-nuget] | [![smb-core-ci][smb-core-ci-nuget-img]][smb-core-nuget] | Core components to provide messaging and file operations capabilities for On-Prem and Cloud environments.
| [SimpleMessageBus.Dispatch][smb-dispatch-nuget]    | [![smb-dispatch-rtm][smb-dispatch-rtm-nuget-img]][smb-dispatch-nuget] | [![smb-dispatch-ci][smb-dispatch-ci-nuget-img]][smb-dispatch-nuget] | Base messaging integration components to accept and direct messages to proper message handler(s).
| [SimpleMessageBus.Hosting][smb-hosting-nuget]    | [![smb-hosting-rtm][smb-hosting-rtm-nuget-img]][smb-hosting-nuget] | [![smb-hosting-ci][smb-hosting-ci-nuget-img]][smb-hosting-nuget] | Configuration components to allow selection of hosting type (i.e: WindowsService vs. Console).
| [SimpleMessageBus.Publish][smb-publish-nuget]    | [![smb-publish-rtm][smb-publish-rtm-nuget-img]][smb-publish-nuget] | [![smb-publish-ci][smb-publish-ci-nuget-img]][smb-publish-nuget] | Base publishing component with FileSystem support to allow messages to be "put" on queues.

#### Azure Integration
| Project | Release | Latest | Description |
|---------|--------|-------------|-------------|
| [SimpleMessageBus.Dispatch.Azure][smb-dispatch-azure-nuget]    | [![smb-dispatch-azure-rtm][smb-dispatch-azure-rtm-nuget-img]][smb-dispatch-azure-nuget] | [![smb-dispatch-azure-ci][smb-dispatch-azure-ci-nuget-img]][smb-dispatch-azure-nuget] | Azure Storage Queue dispatcher with WebJobs SDK integration for cloud-based message processing.
| [SimpleMessageBus.Publish.Azure][smb-publish-azure-nuget]    | [![smb-publish-azure-rtm][smb-publish-azure-rtm-nuget-img]][smb-publish-azure-nuget] | [![smb-publish-azure-ci][smb-publish-azure-ci-nuget-img]][smb-publish-azure-nuget] | Azure Storage Queue publisher for cloud-based message queuing.

#### FileSystem Integration
| Project | Release | Latest | Description |
|---------|--------|-------------|-------------|
| [SimpleMessageBus.Dispatch.FileSystem][smb-dispatch-filesystem-nuget]    | [![smb-dispatch-filesystem-rtm][smb-dispatch-filesystem-rtm-nuget-img]][smb-dispatch-filesystem-nuget] | [![smb-dispatch-filesystem-ci][smb-dispatch-filesystem-ci-nuget-img]][smb-dispatch-filesystem-nuget] | Local and network file system dispatcher for on-premise message processing with enhanced Linux support.

#### Blazor WebAssembly Integration
| Project | Release | Latest | Description |
|---------|--------|-------------|-------------|
| [SimpleMessageBus.IndexedDb.Core][smb-indexeddb-core-nuget]    | [![smb-indexeddb-core-rtm][smb-indexeddb-core-rtm-nuget-img]][smb-indexeddb-core-nuget] | [![smb-indexeddb-core-ci][smb-indexeddb-core-ci-nuget-img]][smb-indexeddb-core-nuget] | Core IndexedDb components for browser-based message storage and management.
| [SimpleMessageBus.Dispatch.IndexedDb][smb-dispatch-indexeddb-nuget]    | [![smb-dispatch-indexeddb-rtm][smb-dispatch-indexeddb-rtm-nuget-img]][smb-dispatch-indexeddb-nuget] | [![smb-dispatch-indexeddb-ci][smb-dispatch-indexeddb-ci-nuget-img]][smb-dispatch-indexeddb-nuget] | IndexedDb dispatcher for browser-based Blazor WebAssembly message processing.
| [SimpleMessageBus.Publish.IndexedDb][smb-publish-indexeddb-nuget]    | [![smb-publish-indexeddb-rtm][smb-publish-indexeddb-rtm-nuget-img]][smb-publish-indexeddb-nuget] | [![smb-publish-indexeddb-ci][smb-publish-indexeddb-ci-nuget-img]][smb-publish-indexeddb-nuget] | IndexedDb publisher for browser-based message queuing in Blazor WebAssembly applications.

### Architecture

<p align="center">
<img src="https://user-images.githubusercontent.com/1657085/54485094-36294e80-4849-11e9-80af-fdc165e60a6d.png">
  <strong>The SimpleMessageBus Architecture</strong>
</p>

**SimpleMessageBus** consists of two main parts: 

### Publish
 - Manages the process of putting "messages" on the queue.
 - Very lightweight, minimal dependencies.
 - Straightforward configuration with Dependency Injection extensions.

### Dispatch
 - Runs in a separate process using the Azure WebJobs SDK.
 - Manages messages coming off the queue and directs them to the proper message handler(s) to be processed.
 - Allows multiple handlers to process the same message.
 - Straightforward configuration with Dependency Injection extensions.
 - Can run in the following environments:
   - Console app (cross-platform with enhanced Linux support)
   - Windows Service
   - Azure WebJobs (Azure's web app offering)
   - Azure Functions (Azure's "serverless" offering)  
   - Blazor WebAssembly (browser-based client-side processing)

### Queues Supported
**SimpleMessageBus** supports multiple backing queue implementations to meet diverse deployment needs:

- **Local File System** - Cross-platform file-based queues with enhanced Linux support for on-premise deployments
- **Azure Storage Queues** - Cloud-based queues with automatic scaling and Azure ecosystem integration  
- **IndexedDb** - Browser-based queues for Blazor WebAssembly applications, enabling client-side message processing
- **HTTP** - RESTful message transmission for distributed and microservice architectures

This flexibility allows applications to run entirely on-premise with no cloud dependencies, in the cloud with automatic scaling, or in the browser for rich client-side experiences, while maintaining the same durability and reliability guarantees across all deployment scenarios.

### Scenarios Supported
**SimpleMessageBus** was designed to streamline the following scenarios:
 - User-created long-latency events (post-registration emails, share notifications, etc)
 - Incoming webhook processing (Stripe, SendGrid, GitHub, etc)
 - Very-long-running tasks (batch processing, cron jobs, etc)

You can read more about these scenarios in our blog post.

## Getting Started
The process of getting **SimpleMessageBus** working in your app is as easy as the name suggests.
  1. Create a new .NET Standard 2.0, .NET 6, .NET 8, or .NET 9 project to that will hold your defined Message types, install the `SimpleMessageBus.Core` NuGet package, and build out 
     your Message types.
  2. Install the appropriate `SimpleMessageBus.Publish.*` NuGet package into your app, reference the library you created in Step 1, and modify your workflows to publish 
     Messages in response to events.
  3. Create a new .NET Standard 2.0, .NET 6, .NET 8, or .NET 9 project that will hold your MessageHandlers, install the `SimpleMessageBus.Core` NuGet package, and build out your 
     MessageHandlers.
  4. Create a new Unit Test project, reference the library you created in Step 3, and test your MessageHandler library with a variety of synthetic Messages.
  5. Create a new Console project, install the appropriate `SimpleMessageBus.Dispatch.*` NuGet package, reference the library you created in Step 3, and inject your 
     MessageHandlers into the DependencyInjection container.

### Sample Projects
Check out our sample projects to see **SimpleMessageBus** in action:
- **[Azure WebJobs Sample](src/SimpleMessageBus.Samples.AzureWebJobs/)** - Demonstrates Azure-based message processing
- **[Blazor WebAssembly Sample](src/SimpleMessageBus.Samples.Blazor.WebAssembly/)** - Shows browser-based messaging with IndexedDb
- **[On-Premise Sample](src/SimpleMessageBus.Samples.OnPrem/)** - File system-based message processing
- **[External Triggers Sample](src/SimpleMessageBus.Samples.ExternalTriggers/)** - Integration with external event sources

## Testing & Quality

**SimpleMessageBus** includes comprehensive testing capabilities:
- **Breakdance Integration** - Enhanced testing framework for reliable message processing verification
- **Unit Test Projects** - Extensive test suites for [Core](src/SimpleMessageBus.Tests.Core/), [Dispatch](src/SimpleMessageBus.Tests.Dispatch/), [Azure](src/SimpleMessageBus.Tests.Dispatch.Azure/), [FileSystem](src/SimpleMessageBus.Tests.Dispatch.FileSystem/), and [Publishing](src/SimpleMessageBus.Tests.Publish/) components
- **Cross-Platform Testing** - Validated on Windows, Linux, and browser environments

## Feedback

Feel free to send us feedback on [Twitter][twitter-link] or [file an issue][issues-link]. Feature requests are always welcome. If you wish to contribute, please take a quick look at the [contribution guidelines](./.github/CONTRIBUTING.md).

## Code of Conduct

Please adhere to our [Code of Conduct](./CODE_OF_CONDUCT.md) during any interactions with 
CloudNimble team members and community members. It is strictly enforced on all official CloudNimble
repositories, websites, and resources. If you encounter someone violating
these terms, please let us know via DM on [Twitter][twitter-link] or via email at opensource@nimbleapps.cloud and we will address it as soon as possible.

## Contributors

Thank you to all the people who have contributed to the project: [Source code Contributors][contri-link]

Please visit our [Contribution](./.github/CONTRIBUTING.md) document to start contributing to our project.


<!-- Base Link References -->

[website-link]: https://nimbleapps.cloud/
[project-link]: https://github.com/CloudNimble/SimpleMessageBus/
[release-link]: https://github.com/CloudNimble/SimpleMessageBus/releases
[doc-link]: https://github.com/CloudNimble/SimpleMessageBus/tree/main/docs
[contri-link]: https://github.com/CloudNimble/SimpleMessageBus/graphs/contributors
[issues-link]: https://github.com/CloudNimble/SimpleMessageBus/issues

[twitter-link]: https://twitter.com/cloud_nimble
[twitter-intent]:https://twitter.com/intent/tweet?via=cloud_nimble&text=Check%20out%20SimpleMessageBus%2C%20the%20framework%20for%20reliable%2C%20distributed%2C%20scalable%2C%20cross-platform%20event%20processing%20on%20.NET.&hashtags=dotnetcore%2Cazure
[twitter-img]:https://img.shields.io/badge/share-on%20twitter-55acee.svg?style=for-the-badge&logo=twitter

<!-- CI/CD Link References -->

[devops-rtm-build]: https://dev.azure.com/cloudnimble/SimpleMessageBus/_build/latest?definitionId=22
[devops-rtm-release]: https://dev.azure.com/cloudnimble/SimpleMessageBus/_release?view=all&definitionId=2

[devops-rtm-build-img]: https://img.shields.io/azure-devops/build/cloudnimble/SimpleMessageBus/22.svg?style=for-the-badge&logo=azuredevops
[devops-rtm-release-img]: https://img.shields.io/azure-devops/release/cloudnimble/202d9877-a3b6-4c67-ae98-768f15eaf6d8/2/2?logo=Azure%20DevOps&style=for-the-badge

<!-- Ecosystem Link References -->

[smb-core-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Core
[smb-dispatch-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Dispatch
[smb-hosting-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Hosting
[smb-publish-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Publish
[smb-dispatch-azure-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Dispatch.Azure
[smb-publish-azure-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Publish.Azure
[smb-dispatch-filesystem-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Dispatch.FileSystem
[smb-indexeddb-core-nuget]: https://www.nuget.org/packages/SimpleMessageBus.IndexedDb.Core
[smb-dispatch-indexeddb-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Dispatch.IndexedDb
[smb-publish-indexeddb-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Publish.IndexedDb

<!-- Badges -->

[smb-core-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Core?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Dispatch?label=&logo=NuGet&style=for-the-badge
[smb-hosting-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Hosting?label=&logo=NuGet&style=for-the-badge
[smb-publish-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Publish?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-azure-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Dispatch.Azure?label=&logo=NuGet&style=for-the-badge
[smb-publish-azure-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Publish.Azure?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-filesystem-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Dispatch.FileSystem?label=&logo=NuGet&style=for-the-badge
[smb-indexeddb-core-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.IndexedDb.Core?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-indexeddb-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Dispatch.IndexedDb?label=&logo=NuGet&style=for-the-badge
[smb-publish-indexeddb-rtm-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Publish.IndexedDb?label=&logo=NuGet&style=for-the-badge

[smb-core-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Core?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Dispatch?label=&logo=NuGet&style=for-the-badge
[smb-hosting-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Hosting?label=&logo=NuGet&style=for-the-badge
[smb-publish-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Publish?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-azure-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Dispatch.Azure?label=&logo=NuGet&style=for-the-badge
[smb-publish-azure-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Publish.Azure?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-filesystem-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Dispatch.FileSystem?label=&logo=NuGet&style=for-the-badge
[smb-indexeddb-core-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.IndexedDb.Core?label=&logo=NuGet&style=for-the-badge
[smb-dispatch-indexeddb-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Dispatch.IndexedDb?label=&logo=NuGet&style=for-the-badge
[smb-publish-indexeddb-ci-nuget-img]: https://img.shields.io/nuget/vpre/SimpleMessageBus.Publish.IndexedDb?label=&logo=NuGet&style=for-the-badge
