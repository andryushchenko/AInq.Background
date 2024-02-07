# AInq.Background

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/andryushchenko/AInq.Background)](https://github.com/andryushchenko/AInq.Background/releases) [![GitHub](https://img.shields.io/github/license/andryushchenko/AInq.Background)](LICENSE)

![AInq](https://raw.githubusercontent.com/andryushchenko/AInq.Background/main/AInq.png)

## What is it?

Background work utilities for .NET Core apps based on Hosted services. Originally designed for accessing API with strict request-per-second limit.

- **Background work queue** with configurable parallelism and optional prioritizing
- **Shared resource access queue** with different resource reuse strategies and optional prioritizing 
- **Background data processing conveyor** with different conveyor machine reuse strategies and optional prioritizing
- **Work scheduler** with Cron support
- **Startup work** utility

## New in 4.1

- **BREAKING CHANGES**
  - **Startup work** utility moved to separate package **AInq.Background.Startup**
- Minor bug fix and internal optimization

## New in 4.0

- **BREAKING CHANGES**
  - Background work Scheduler interfaces moved to separate package **AInq.Background.Scheduler.Abstraction**
  - Background work Scheduler now uses `Try` and `Maybe` from **AInq.Optional** to pass errors logically correct to Observable
- Background work Scheduler now uses **System.Reactive** instead of custom buggy `IObservable<T>` implementation

## New in 3.0

- **GENERAL BUGFIX**
- **New features**
  - Service interaction extensions
  - Batch processing extension
  - Repeated work in `IWorkScheduler`
  - Work reslts in `IWorkScheduler`
- **Refactoring**
  - Simplify basic interfaces: non-basic methods moved to extensions
  - Cleanup Helers and Extensions classes struct
- **BREAKING CHANGES**
  - Removed some unsued methods from `WorkFactory` and `AccessFactory`
  - Some extension methods moved form `AInq.Background.Helpers` namespace to `AInq.Background.Extensions` and `AInq.Background.Interaction`
  - ~~`IActivatable`~~ `IStartStopppable`
 

## Packages description
#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Abstraction)](https://www.nuget.org/packages/AInq.Background.Abstraction/) AInq.Background.Abstraction

Basic interfaces and helpers library.

- Basic interfaces and factory classes:
  - Work interfaces: `IWork`, `IWork<TResult>`, `IAsyncWork`, `IAsyncWork<TResult>`
  - `WorkFactory` for creating simple work instances from delegates
  - Resource access interfaces: `IAccess<TResource>`, `IAccess<TResource, TResult>`, `IAsyncAccess<TResource>`, `IAsyncAccess<TResource, TResult>`
  - `AccessFactory` for creating simple access instances from delegates
  - `IConveyorMachine<TData, TResult>` for conveyor data processing machines
  - `IActivatable` and `IThrottling` for shared resources and conveyor machines with particular usage strategies
- Service interfaces
  - `IWorkQueue` and `IPriorityWorkQueue` for background task queue
  - `IAccessQueue<TResource>` and `IPriorityAccessQueue<TResource>` for shared resource access queue
  - `IConveyor<TData, TResult>` and `IPriorityConveyor<TData, TResult>` for background data processing conveyor
- Helpers and extensions including methods to use services together (eg. enqueue `IAccess<TResource>` to `IWorkQueue`) if needed

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

You can extend functionality by implementing custom `ITaskWrapper`, `ITaskManager` or `ITaskProcessor` and combine with existing ones to create more service variants. 

#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Scheduler.Abstraction)](https://www.nuget.org/packages/AInq.Background.Scheduler.Abstraction/) AInq.Background.Scheduler.Abstraction

Work scheduler interfaces and helpers library.

- Service interfaces
  - `IWorkScheduler` for background work scheduler
- Helpers and extensions including methods to use services together (eg. schedule `IAccess<TResource>` to `IWorkScheduler`) if needed

#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Scheduler)](https://www.nuget.org/packages/AInq.Background.Scheduler/) AInq.Background.Scheduler

Work scheduler implementation.
- Support delayed, time-scheduled, and cron-scheduled work
- Use `WorkSchedulerInjection` to regiter service or create for internal usage

You can extend functionality by implementing custom `IScheduledTaskWrapper` or `IWorkSchedulerManager` and combine with existing ones to create more service variants. 

**NOTE:** [Cronos](https://github.com/HangfireIO/Cronos) is used for parsing Cron expressions - follow documentation for supported options. Format with seconds is supported.

#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Enumerable)](https://www.nuget.org/packages/AInq.Background.Enumerable/) AInq.Background.Enumerable

**NEW** Batch processing extensions for `IWorkQueue`, `IAccessQueue<TResource>` and `IConveyor<TData, TResult>`

#### [![Nuget](https://img.shields.io/nuget/v/AInq.Background.Startup)](https://www.nuget.org/packages/AInq.Background.Startup/) AInq.Background.Startup

**NEW** Startup work utility for running some work *before* host start

Support interaction with background work queue

## Documentation

As for now documentation is provided in this document and by XML documentation inside packages.

## Contribution

These packages are in active production use, all fixes and improvements will be published after some internal testing.

If you find a bug, have a question or something else - you are friendly welcome to open an issue.

## License
Copyright © 2020 Anton Andryushchenko. AInq.Background is licensed under [Apache License 2.0](LICENSE)
