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

using static AInq.Background.Tasks.QueuedWorkFactory;

namespace AInq.Background.Interaction;

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled work in background queue </summary>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkSchedulerWorkQueueInteraction
{
#region DelayedQueue

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan delay, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            delay,
            cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork, TimeSpan delay,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            delay,
            cancellation);

#endregion

#region DelayedQueueDI

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork>(priority, attemptsCount), delay, cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork, TResult>(priority, attemptsCount), delay, cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork>(priority, attemptsCount), delay, cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(priority, attemptsCount), delay, cancellation);

#endregion

#region ScheduledQueue

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, DateTime time, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            time,
            cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            time,
            cancellation);

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            time,
            cancellation);

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork, DateTime time,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            time,
            cancellation);

#endregion

#region ScheduledQueueDI

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, DateTime time, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork>(priority, attemptsCount), time, cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork, TResult>(priority, attemptsCount), time, cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime time, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork>(priority, attemptsCount), time, cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime time, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(priority, attemptsCount), time, cancellation);

#endregion

#region CronQueue

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueWork(this IWorkScheduler scheduler, IWork work, string cronExpression, int priority = 0,
        int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork,
        string cronExpression, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

#endregion

#region CronQueueDI

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueWork<TWork>(this IWorkScheduler scheduler, string cronExpression, int priority = 0,
        int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedWork<TWork>(priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueWork<TWork, TResult>(this IWorkScheduler scheduler, string cronExpression, int priority = 0,
        int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedWork<TWork, TResult>(priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork>(priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, string cronExpression,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(priority, attemptsCount),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

#endregion

#region RepeatedScheduledQueue

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IWorkScheduler scheduler, IWork work, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork,
        DateTime starTime, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

#endregion

#region RepeatedScheduledQueueDI

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(CreateQueuedInjectedWork<TWork>(priority, attemptsCount), starTime, repeatDelay, execCount, cancellation);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedWork<TWork, TResult>(priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork>(priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(priority, attemptsCount),
            starTime,
            repeatDelay,
            execCount,
            cancellation);

#endregion

#region RepeatedDelayedQueue

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork,
        TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

#endregion

#region RepeatedDelayedQueueDI

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(CreateQueuedInjectedWork<TWork>(priority, attemptsCount), startDelay, repeatDelay, execCount, cancellation);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedWork<TWork, TResult>(priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork>(priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(priority, attemptsCount),
            startDelay,
            repeatDelay,
            execCount,
            cancellation);

#endregion
}
