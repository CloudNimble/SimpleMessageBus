<h1 align="center">SimpleMessageBus - By CloudNimble</h1>
<h4 align="center">A framework for reliable, distributed, scalable, cross-platform event processing on .NET.</h4>

<div align="center">
  
[Releases](https://github.com/CloudNimble/SimpleMessageBus/releases)&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;Documentation&nbsp;&nbsp;&nbsp;

[![Build Status][devops-build-img]][devops-build] [![Release Status][devops-release-img]][devops-release] [![Twitter][twitter-img]][twitter-intent]

</div>

## What is SimpleMessageBus?
**SimpleMessageBus** is a system for making applications more reliable and responsive to users by processing potentially long-running tasks out-of-band from
the user's main workflow. It is designed to run either on-prem, or in the Microsoft Cloud, making it suitable for any application, and able to grow as 
your needs do.

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
   - Azure WebJobs
   - Azure Functions

### Queues Supported
SimpleMessageBus currently supports two backing queues: the local file system, and Azure Storage Queues. This allows the Dispatcher to run entirely on-prem
with no Azure dependencies, but still get the same durability and reliability through local poison queues and completed event storage.

<!--
Link References
-->

[devops-build]:https://dev.azure.com/cloudnimble/SimpleMessageBus/_build/latest?definitionId=11
[devops-release]:https://dev.azure.com/cloudnimble/SimpleMessageBus/_release?view=all&definitionId=1
[nightly-feed]:https://www.myget.org/F/restier-nightly/api/v3/index.json
[twitter-intent]:https://twitter.com/intent/tweet?via=robertmclaws&text=Check%20out%20SimpleMessageBus%2C%20the%20framework%20for%20reliable%2C%20distributed%2C%20scalable%2C%20cross-platform%20event%20processing%20on%20.NET.&hashtags=dotnetcore%2Cazure

[devops-build-img]:https://img.shields.io/azure-devops/build/cloudnimble/simplemessagebus/11.svg?style=for-the-badge&logo=azuredevops
[devops-release-img]:https://img.shields.io/azure-devops/release/cloudnimble/202d9877-a3b6-4c67-ae98-768f15eaf6d8/1/1.svg?style=for-the-badge&logo=azuredevops
[nightly-feed-img]:https://img.shields.io/badge/continuous%20integration-feed-0495dc.svg?style=for-the-badge&logo=nuget&logoColor=fff
[github-version-img]:https://img.shields.io/github/release/ryanoasis/nerd-fonts.svg?style=for-the-badge
[twitter-img]:https://img.shields.io/badge/share-on%20twitter-55acee.svg?style=for-the-badge&logo=twitter
