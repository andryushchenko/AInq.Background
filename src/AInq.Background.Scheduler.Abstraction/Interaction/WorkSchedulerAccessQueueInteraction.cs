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

using static AInq.Background.Tasks.QueuedAccessFactory;

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
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IWorkScheduler scheduler, IAccess<TResource, TResult> access,
        TimeSpan delay, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> asyncAccess, TimeSpan delay,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> asyncAccess, TimeSpan delay, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            delay,
            cancellation);

#endregion

#region DelayedAccessDI

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess>(attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess, TResult>(attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(attemptsCount, priority),
            delay,
            cancellation);

#endregion

#region ScheduledAccess

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IWorkScheduler scheduler, IAccess<TResource, TResult> access,
        DateTime time, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled asynchronous queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> asyncAccess, DateTime time,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled asynchronous queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> asyncAccess, DateTime time, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            time,
            cancellation);

#endregion

#region ScheduledAccessDI

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess>(attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess, TResult>(attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler, DateTime time,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(attemptsCount, priority),
            time,
            cancellation);

#endregion

#region CronAccess

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TResult>(this IWorkScheduler scheduler, IAccess<TResource, TResult> access,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource>(this IWorkScheduler scheduler, IAsyncAccess<TResource> asyncAccess,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> asyncAccess, string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

#endregion

#region CronAccessDI

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess, TResult>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access scheduler instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

#endregion

#region RepeatedScheduledAccess

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource> asyncAccess, DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> asyncAccess, DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1,
        int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

#endregion

#region RepeatedScheduledAccessDI

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess>(attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess, TResult>(attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(attemptsCount, priority),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

#endregion

#region RepeatedDelayedAccess

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IWorkScheduler scheduler, IAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource> asyncAccess, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="asyncAccess"> Async access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IWorkScheduler scheduler,
        IAsyncAccess<TResource, TResult> asyncAccess, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1,
        int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

#endregion

#region RepeatedDelayedAccessDI

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAccess<TResource, TAccess, TResult>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IWorkScheduler scheduler,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous access to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Access Scheduler instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IWorkScheduler scheduler,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

#endregion
}
