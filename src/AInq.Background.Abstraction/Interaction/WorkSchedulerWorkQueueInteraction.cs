// Copyright 2020 Anton Andryushchenko
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

using AInq.Background.Extensions;
using AInq.Background.Helpers;
using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Tasks.WorkFactory;

namespace AInq.Background.Interaction
{

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled work in background queue  </summary>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkSchedulerWorkQueueInteraction
{
    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            delay,
            cancellation);
    }

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork, TResult>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancel, attemptsCount, priority)),
            delay,
            cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            time,
            cancellation);
    }

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork, TResult>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancel, attemptsCount, priority)),
            time,
            cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="scheduler" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddCronQueueWork(this IWorkScheduler scheduler, IWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

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
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="scheduler" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddCronQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="scheduler" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddCronAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="scheduler" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddCronAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
    {
        _ = work ?? throw new ArgumentNullException(nameof(work));
        return (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            cronExpression,
            cancellation,
            execCount);
    }

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddCronQueueWork<TWork>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork>(cancel, attemptsCount, priority)),
            cronExpression,
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddCronQueueWork<TWork, TResult>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork, TResult>(cancel, attemptsCount, priority)),
            cronExpression,
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddCronAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork>(cancel, attemptsCount, priority)),
            cronExpression,
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception><exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddCronAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancel, attemptsCount, priority)),
            cronExpression,
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
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedQueueWork(this IWorkScheduler scheduler, IWork work, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedQueueWork<TWork>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork, TResult>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork>(cancel, attemptsCount, priority)),
            starTime,
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancel, attemptsCount, priority)),
            starTime,
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
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork(work, cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork(work, cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork>(cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueWork<TWork, TResult>(cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<object?> AddRepeatedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork>(cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
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
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<TResult> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancel, attemptsCount, priority)),
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);
}

}
