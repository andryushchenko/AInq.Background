# AInq.Background

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/andryushchenko/AInq.Background)](https://github.com/andryushchenko/AInq.Background/releases) [![netstandard 2.0](https://img.shields.io/badge/netstandard-2.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) [![GitHub](https://img.shields.io/github/license/andryushchenko/AInq.Background)](LICENSE)

## What is it?

Background work utilities for .NET Core apps based on Hosted services. Originally designed for accessing API with strict request-per-second limit.

- **Background work queue** with configurable parallelism and optional prioritizing
- **Shared resource access queue** with different resource reuse strategies and optional prioritizing 
- **Background data processing conveyor** with different conveyor machine reuse strategies and optional prioritizing
- **Work scheduler** with Cron support
- **Startup work** utility

## Packages description
#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Abstraction)](https://www.nuget.org/packages/AInq.Background.Abstraction/) AInq.Background.Abstraction

Abstraction library with no additional dependencies.

- Basic interfaces and factory classes:
    - Work interfaces: `IWork`, `IWork<TResult>`, `IAsyncWork`, `IAsyncWork<TResult>`
    - `WorkFactory` for creating simple work instances from delegates
    - Resource access interfaces: `IAccess<TResource>`, `IAccess<TResource, TResult>`, `IAsyncAccess<TResource>`, `IAsyncAccess<TResource, TResult>`
    - `AccessFactory` for creating simple access instances from delegates
    - `IConveyorMachine<TData, TResult>` for conveyor data processing machines
    - `IActivatable` and `IThrottling` for shared resources and conveyor machines with particular usage strategies
- Service interfaces and helpers
    - `IWorkQueue` and `IPriorityWorkQueue` for background task queue with `WorkQueueHelper`
    - `IAccessQueue<TResource>` and `IPriorityAccessQueue<TResource>` for shared resource access queue with `AccessQueueHelper`
    - `IConveyor<TData, TResult>` and `IPriorityConveyor<TData, TResult>`for background data processing conveyor
    - `IWorkScheduler` for work scheduler with `WorkSchedulerHelper` and `WorkSchedulerQueueHelper` extension for interaction with Work Queue

#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background)](https://www.nuget.org/packages/AInq.Background/) AInq.Background

Queues and conveyor implementations.

- Background work queue
    - Optional support for configurable parallelism
    - Optional support for prioritizing
    - Use `WorkQueueInjection` to regiter service or create for internal usage
- Shared resource access queue
    - Support single or many resource instances with different lifetime
    - Optional support for prioritizing
    - Use `AccessQueueInjection` to regiter service or create for internal usage
- Background data processing conveyor
    - Support single or many conveyor machines with different lifetime
    - Optional support for prioritizing
    - Use `ConveyorInjection` to regiter service or create for internal usage
- Startup work utility for running some work *before* host start
    - Supports interaction with background work queue
    - Use `StartupWorkInjection` to register and run works

#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Scheduler)](https://www.nuget.org/packages/AInq.Background.Scheduler/) AInq.Background.Scheduler

Work scheduler implementation.

- Supports delayed, time-scheduled, and cron-scheduled work
- Use `WorkSchedulerInjection` to regiter service or create for internal usage

**NOTE:** [Cronos](https://github.com/HangfireIO/Cronos) is used for parsing Cron expressions - follow documentation for supported options.

## Documentation

As for now documentation is provided in this document and by XML documentation inside packages.

## Contribution

These packages are in active production use, all fixes and improvements will be published after some internal testing.

If you find a bug, have a question or something else - you are friendly welcome to open an issue.

## License
[Apache-2.0](LICENSE)

