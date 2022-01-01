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

using static AInq.Background.Tasks.WorkFactory;

namespace AInq.Background.Interaction;

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled access in background queue </summary>
/// <remarks> <see cref="IPriorityAccessQueue{TResource}" /> or <see cref="IAccessQueue{TResource}" /> service should be registered on host to run queued access </remarks>
public static class WorkSchedulerAccessQueueInteraction
{
#region DelayedAccess

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task AddScheduledQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IWorkScheduler scheduler, IAccess<TResource, TResult> access,
        TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> access, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> access, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

#endregion

#region DelayedAccessDI

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess, TResult>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(CreateAsyncWork((provider, cancel)
                => provider.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

#endregion

#region ScheduledAccess

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task AddScheduledQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IWorkScheduler scheduler, IAccess<TResource, TResult> access,
        DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled asynchronous queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> access, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled asynchronous queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> access, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

#endregion

#region ScheduledAccessDI

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess, TResult>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(CreateAsyncWork((provider, cancel)
                => provider.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancel, attemptsCount, priority)),
            time,
            cancellation);

#endregion

#region CronAccess

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TResult>(this IWorkScheduler scheduler, IAccess<TResource, TResult> access,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> access,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> access, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

#endregion

#region CronAccessDI

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
    {
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess>(cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
    {
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess, TResult>(cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
    {
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess>(cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
    {
        _ = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

#endregion

#region RepeatedScheduledAccess

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);
    }

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);
    }

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);
    }

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);
    }

#endregion

#region RepeatedScheduledAccessDI

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess, TResult>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

#endregion

#region RepeatedDelayedAccess

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);
    }

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess(access, cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);
    }

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);
    }

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
    {
        _ = access ?? throw new ArgumentNullException(nameof(access));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess(access, cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);
    }

#endregion

#region RepeatedDelayedAccessDI

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess>(cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAccess<TResource, TAccess, TResult>(cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess>(cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancel, attemptsCount, priority)),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

#endregion
}
