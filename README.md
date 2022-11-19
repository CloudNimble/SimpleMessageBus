<h1 align="center">SimpleMessageBus - By CloudNimble</h1>
<h4 align="center">A framework for reliable, distributed, scalable, cross-platform event processing on .NET.</h4>

<div align="center">

[Releases][release-link]&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;[Documentation][doc-link]&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp; Site &nbsp;&nbsp;&nbsp;

[![Build Status][devops-build-img]][devops-build] [![Release Status][devops-release-img]][devops-release] [![Twitter][twitter-img]][twitter-intent]

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

| Project | Status | Description |
|---------|--------|-------------|
| [SimpleMessageBus.Core][smb-core-nuget]    | [![smb-core][smb-core-nuget-img]][smb-core-nuget] | Core components to provide messaging and file operations capabilities for On-Prem and Cloud environments.
| [SimpleMessageBus.Dispatch][smb-dispatch-nuget]    | [![smb-dispatch][smb-dispatch-nuget-img]][smb-dispatch-nuget] | Messaging integration components with Azure WebJobs SDK dependency to accept and direct messages to proper message handler(s).
| [SimpleMessageBus.Hosting][smb-hosting-nuget]    | [![smb-hosting][smb-hosting-nuget-img]][smb-hosting-nuget] | Configuration components to allow selection of hosting type (i.e: WindowsService vs. Console).
| [SimpleMessageBus.Publish][smb-publish-nuget]    | [![smb-publish][smb-publish-nuget-img]][smb-publish-nuget] | Publishing integration component with Azure Queue Storag dependency to allow messages to be "put" on queues

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
   - Console app (cross platform)
   - Windows Service
   - Azure WebJobs (Azure's web app offering)
   - Azure Functions (Azure's "serverless" offering)

### Queues Supported
**SimpleMessageBus** currently supports two backing queues: the **local file system**, and **Azure Storage Queues**. This allows the Dispatcher to run entirely 
on-prem with no Azure dependencies, but still get the same durability and reliability through local poison queues and completed event storage.

### Scenarios Supported
**SimpleMessageBus** was designed to streamline the following scenarios:
 - User-created long-latency events (post-registration emails, share notifications, etc)
 - Incoming webhook processing (Stripe, SendGrid, GitHub, etc)
 - Very-long-running tasks (batch processing, cron jobs, etc)

You can read more about these scenarios in our blog post.

## Getting Started
The process of getting **SimpleMessageBus** working in your app is as easy as the name suggests.
  1. Create a new .NET Standard 2.0, .NET 6 or .NET 7 project to that will hold your defined Message types, install the `SimpleMessageBus.Core` NuGet package, and build out 
     your Message types.
  2. Install the `SimpleMessageBus.Publish` NuGet package into your app, reference the library you created in Step 1, and modify your workflows to publish 
     Messages in response to events.
  3. Create a new .NET Standard 2.0, .NET 6 or .NET 7 project that will hold your MessageHandlers, install the `SimpleMessageBus.Core` NuGet package, and build out your 
     MessageHandlers.
  4. Create a new Unit Test project, reference the library you created in Step 3, and test your MessageHandler library with a variety of synthetic Messages.
  5. Create a new Console project, install the `SimpleMessageBus.Dispatch` NuGet package, reference the library you created in Step 3, and inject your 
     MessageHandlers into the DependencyInjection container.

## Feedback

Feel free to send us feedback on [Twitter][twitter-link] or [file an issue][issues-link]. Feature requests are always welcome. If you wish to contribute, please take a quick look at the [guidelines](./CONTRIBUTING.md)!

## Code of Conduct

Please adhere to our [Code of Conduct](./CODE_OF_CONDUCT.md) during any interactions with 
CloudNimble team members and community members. It is strictly enforced on all official CloudNimble
repositories, websites, and resources. If you encounter someone violating
these terms, please let us know via DM on [Twitter][twitter-link] or via email at opensource@nimbleapps.cloud and we will address it as soon as possible.

## Contributors

Thank you to all the people who have contributed to the project: [Source code Contributors][contri-link]

Please visit our [Contribution](./.github/CONTRIBUTING.md) document to start contributing to our project.

<!-- Base Link References -->
[project-link]: https://github.com/CloudNimble/SimpleMessageBus/
[release-link]: https://github.com/CloudNimble/SimpleMessageBus/releases
[doc-link]: https://github.com/CloudNimble/SimpleMessageBus/tree/main/docs
[contri-link]: https://github.com/CloudNimble/SimpleMessageBus/graphs/contributors
[issues-link]: https://github.com/CloudNimble/SimpleMessageBus/issues

[twitter-link]: https://twitter.com/cloud_nimble
[twitter-intent]:https://twitter.com/intent/tweet?via=cloud_nimble&text=Check%20out%20SimpleMessageBus%2C%20the%20framework%20for%20reliable%2C%20distributed%2C%20scalable%2C%20cross-platform%20event%20processing%20on%20.NET.&hashtags=dotnetcore%2Cazure
[twitter-img]:https://img.shields.io/badge/share-on%20twitter-55acee.svg?style=for-the-badge&logo=twitter

<!-- CI/CD Link References -->

[devops-build]:https://dev.azure.com/cloudnimble/SimpleMessageBus/_build/latest?definitionId=11
[devops-release]:https://dev.azure.com/cloudnimble/SimpleMessageBus/_release?view=all&definitionId=1

[devops-build-img]:https://img.shields.io/azure-devops/build/cloudnimble/simplemessagebus/11.svg?style=for-the-badge&logo=azuredevops
[devops-release-img]:https://img.shields.io/azure-devops/release/cloudnimble/202d9877-a3b6-4c67-ae98-768f15eaf6d8/1/1.svg?style=for-the-badge&logo=azuredevops

<!--
Ecosystem Link References
-->

[smb-core-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Core
[smb-dispatch-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Dispatch
[smb-hosting-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Hosting
[smb-publish-nuget]: https://www.nuget.org/packages/SimpleMessageBus.Publish

[smb-core-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Core?label=NuGet&logo=NuGet&style=for-the-badge
[smb-dispatch-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Dispatch?label=NuGet&logo=NuGet&style=for-the-badge
[smb-hosting-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Hosting?label=NuGet&logo=NuGet&style=for-the-badge
[smb-publish-nuget-img]: https://img.shields.io/nuget/v/SimpleMessageBus.Publish?label=NuGet&logo=NuGet&style=for-the-badge