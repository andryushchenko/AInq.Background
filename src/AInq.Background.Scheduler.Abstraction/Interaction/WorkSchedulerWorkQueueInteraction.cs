﻿// Copyright 2020-2023 Anton Andryushchenko
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
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            delay,
            cancellation);

#endregion

#region DelayedQueueDI

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork>(attemptsCount, priority), delay, cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork, TResult>(attemptsCount, priority), delay, cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork>(attemptsCount, priority), delay, cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(attemptsCount, priority), delay, cancellation);

#endregion

#region ScheduledQueue

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            time,
            cancellation);

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            time,
            cancellation);

#endregion

#region ScheduledQueueDI

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork>(attemptsCount, priority), time, cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedWork<TWork, TResult>(attemptsCount, priority), time, cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork>(attemptsCount, priority), time, cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(attemptsCount, priority), time, cancellation);

#endregion

#region CronQueue

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueWork(this IWorkScheduler scheduler, IWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

#endregion

#region CronQueueDI

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronQueueWork<TWork>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedWork<TWork>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronQueueWork<TWork, TResult>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedWork<TWork, TResult>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(attemptsCount, priority),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            cancellation,
            execCount);

#endregion

#region RepeatedScheduledQueue

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IWorkScheduler scheduler, IWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

#endregion

#region RepeatedScheduledQueueDI

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(CreateQueuedInjectedWork<TWork>(attemptsCount, priority), starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedWork<TWork, TResult>(attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork>(attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(attemptsCount, priority),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

#endregion

#region RepeatedDelayedQueue

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork asyncWork, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> asyncWork,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

#endregion

#region RepeatedDelayedQueueDI

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(CreateQueuedInjectedWork<TWork>(attemptsCount, priority), startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedWork<TWork, TResult>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(attemptsCount, priority),
            startDelay,
            repeatDelay,
            cancellation,
            execCount);

#endregion
}
