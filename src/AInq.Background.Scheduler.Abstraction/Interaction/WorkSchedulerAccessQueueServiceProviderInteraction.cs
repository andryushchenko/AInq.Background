// Copyright 2020-2023 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace AInq.Background.Interaction;

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled access in registered background queue </summary>
/// <remarks> <see cref="IWorkScheduler" /> service should be registered on host to schedule work </remarks>
/// <remarks> <see cref="IPriorityAccessQueue{TResource}" /> or <see cref="IAccessQueue{TResource}" /> service should be registered on host to run queued access </remarks>
public static class WorkSchedulerAccessQueueServiceProviderInteraction
{
#region DelayedAccess

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},TimeSpan,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), delay, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},TimeSpan,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        TimeSpan delay, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), delay, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},TimeSpan,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access, TimeSpan delay,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), delay, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},TimeSpan,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, TimeSpan delay, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), delay, priority, attemptsCount, cancellation);

#endregion

#region DelayedAccessDI

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess}(IWorkScheduler,TimeSpan,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IServiceProvider provider, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess<TResource, TAccess>(delay, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess<TResource, TAccess, TResult>(delay, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,TimeSpan,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(delay, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider, TimeSpan delay,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(delay, priority, attemptsCount, cancellation);

#endregion

#region ScheduledAccess

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), time, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        DateTime time, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), time, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access, DateTime time,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), time, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, DateTime time, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)), time, priority, attemptsCount, cancellation);

#endregion

#region ScheduledAccessDI

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IServiceProvider provider, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess<TResource, TAccess>(time, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueAccess<TResource, TAccess, TResult>(time, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(time, priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider, DateTime time,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(time, priority, attemptsCount, cancellation);

#endregion

#region CronAccess

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource}" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},string,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource}" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},string,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

#endregion

#region CronAccessDI

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource,TAccess}(IWorkScheduler,string,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource, TAccess>(this IServiceProvider provider, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueAccess<TResource, TAccess>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource,TAccess,TResult}" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueAccess<TResource, TAccess, TResult>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,string,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueAccess<TResource, TAsyncAccess>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource,TAsyncAccess,TResult}" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueAccess<TResource, TAsyncAccess, TResult>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

#endregion

#region RepeatedScheduledAccess

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               starTime,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               starTime,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               starTime,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               starTime,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

#endregion

#region RepeatedScheduledAccessDI

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess<TResource, TAccess>(starTime, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess,TResult}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess<TResource, TAccess, TResult>(starTime, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(starTime, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess,TResult}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(starTime, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

#endregion

#region RepeatedDelayedAccess

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               startDelay,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               startDelay,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               startDelay,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1,
        int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess(access ?? throw new ArgumentNullException(nameof(access)),
               startDelay,
               repeatDelay,
               priority: priority,
               attemptsCount: attemptsCount,
               execCount: execCount,
               cancellation: cancellation);

#endregion

#region RepeatedDelayedAccessDI

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess<TResource, TAccess>(startDelay, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess,TResult}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueAccess<TResource, TAccess, TResult>(startDelay, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(startDelay, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

    /// <inheritdoc cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess,TResult}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(startDelay, repeatDelay, priority: priority, attemptsCount: attemptsCount, execCount: execCount, cancellation: cancellation);

#endregion
}
