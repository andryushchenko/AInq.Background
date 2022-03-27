// Copyright 2020-2022 Anton Andryushchenko
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

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled work in registered background queue </summary>
/// <remarks> <see cref="IWorkScheduler" /> service should be registered on host to schedule work </remarks>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkSchedulerWorkQueueServiceProviderInteraction
{
#region DelayedQueue

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork(IWorkScheduler,IWork,TimeSpan,CancellationToken,int,int)" />
    [PublicAPI]
    public static Task AddScheduledQueueWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TResult}(IWorkScheduler, IWork{TResult}, TimeSpan, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork(IWorkScheduler,IAsyncWork,TimeSpan,CancellationToken,int,int)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, TimeSpan, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation, attemptsCount, priority);

#endregion

#region DelayedQueueDI

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task AddScheduledQueueWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork<TWork>(delay, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork,TResult}(IWorkScheduler,DateTime,CancellationToken,int,int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork<TWork, TResult>(delay, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork<TAsyncWork>(delay, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork,TResult}(IWorkScheduler,DateTime,CancellationToken,int,int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(delay, cancellation, attemptsCount, priority);

#endregion

#region ScheduledQueue

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork(IWorkScheduler, IWork, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task AddScheduledQueueWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TResult}(IWorkScheduler, IWork{TResult}, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork(IWorkScheduler, IAsyncWork, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation, attemptsCount, priority);

#endregion

#region ScheduledQueueDI

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task AddScheduledQueueWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork<TWork>(time, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledQueueWork<TWork, TResult>(time, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork<TAsyncWork>(time, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(time, cancellation, attemptsCount, priority);

#endregion

#region CronQueue

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork(IWorkScheduler, IWork, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueWork(this IServiceProvider provider, IWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TResult}(IWorkScheduler, IWork{TResult}, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork(IWorkScheduler, IAsyncWork, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

#endregion

#region CronQueueDI

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TWork}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueWork<TWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueWork<TWork>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TWork, TResult}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueWork<TWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronQueueWork<TWork, TResult>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TAsyncWork}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueWork<TAsyncWork>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncQueueWork<TAsyncWork, TResult>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               attemptsCount,
               priority,
               execCount);

#endregion

#region RepeatedScheduledQueue

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork(IWorkScheduler,IWork,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IServiceProvider provider, IWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               starTime,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TResult}(IWorkScheduler,IWork{TResult},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               starTime,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork(IWorkScheduler,IAsyncWork,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               starTime,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               starTime,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

#endregion

#region RepeatedScheduledQueueDI

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IServiceProvider provider, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork<TWork>(starTime, repeatDelay, cancellation, attemptsCount, priority, execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork<TWork, TResult>(starTime, repeatDelay, cancellation, attemptsCount, priority, execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork<TAsyncWork>(starTime, repeatDelay, cancellation, attemptsCount, priority, execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(starTime, repeatDelay, cancellation, attemptsCount, priority, execCount);

#endregion

#region RepeatedDelayedQueue

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork(IWorkScheduler,IWork,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IServiceProvider provider, IWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               startDelay,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TResult}(IWorkScheduler,IWork{TResult},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               startDelay,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork(IWorkScheduler,IAsyncWork,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               startDelay,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
               startDelay,
               repeatDelay,
               cancellation,
               attemptsCount,
               priority,
               execCount);

#endregion

#region RepeatedDelayedQueueDI

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IServiceProvider provider, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork<TWork>(startDelay, repeatDelay, cancellation, attemptsCount, priority, execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedQueueWork<TWork, TResult>(startDelay, repeatDelay, cancellation, attemptsCount, priority, execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork<TAsyncWork>(startDelay, repeatDelay, cancellation, attemptsCount, priority, execCount);

    /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(startDelay, repeatDelay, cancellation, attemptsCount, priority, execCount);

#endregion
}
