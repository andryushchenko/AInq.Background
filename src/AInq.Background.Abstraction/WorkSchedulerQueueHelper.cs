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

using System;
using System.Threading;
using static AInq.Background.WorkFactory;

namespace AInq.Background
{

/// <summary> <see cref="IWorkScheduler"/> extensions to run scheduled work in background queue  </summary>
/// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
public static class WorkSchedulerQueueHelper
{
    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedQueueWork(this IWorkScheduler scheduler, IWork work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueWork(work, cancellation, attemptsCount, priority).Ignore()), delay, cancellation);
    }

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueWork(work, cancellation, attemptsCount, priority).Ignore()), delay, cancellation);
    }

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueAsyncWork(work, cancellation, attemptsCount, priority).Ignore()), delay, cancellation);
    }

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueAsyncWork(work, cancellation, attemptsCount, priority).Ignore()), delay, cancellation);
    }

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedQueueWork<TWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueWork<TWork>(cancellation, attemptsCount, priority).Ignore()), delay, cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedQueueWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueWork<TWork, TResult>(cancellation, attemptsCount, priority).Ignore()), delay, cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork>(cancellation, attemptsCount, priority).Ignore()), delay, cancellation);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    public static void AddDelayedAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => scheduler.AddDelayedWork(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, attemptsCount, priority).Ignore()), delay, cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledQueueWork(this IWorkScheduler scheduler, IWork work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueWork(work, cancellation, attemptsCount, priority).Ignore()), time, cancellation);
    }

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueWork(work, cancellation, attemptsCount, priority).Ignore()), time, cancellation);
    }

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueAsyncWork(work, cancellation, attemptsCount, priority).Ignore()), time, cancellation);
    }

    /// <summary> Add scheduled asynchronous queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueAsyncWork(work, cancellation, attemptsCount, priority).Ignore()), time, cancellation);
    }

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledQueueWork<TWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueWork<TWork>(cancellation, attemptsCount, priority).Ignore()), time, cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledQueueWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueWork<TWork, TResult>(cancellation, attemptsCount, priority).Ignore()), time, cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork>(cancellation, attemptsCount, priority).Ignore()), time, cancellation);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    public static void AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => scheduler.AddScheduledWork(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, attemptsCount, priority).Ignore()), time, cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronQueueWork(this IWorkScheduler scheduler, IWork work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddCronWork(CreateWork(provider => provider.EnqueueWork(work, cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);
    }

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronQueueWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddCronWork(CreateWork(provider => provider.EnqueueWork(work, cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);
    }

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronAsyncQueueWork(this IWorkScheduler scheduler, IAsyncWork work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddCronWork(CreateWork(provider => provider.EnqueueAsyncWork(work, cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);
    }

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronAsyncQueueWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        if (work == null)
            throw new ArgumentNullException(nameof(work));
        scheduler.AddCronWork(CreateWork(provider => provider.EnqueueAsyncWork(work, cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);
    }

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronQueueWork<TWork>(this IWorkScheduler scheduler, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => scheduler.AddCronWork(CreateWork(provider => provider.EnqueueWork<TWork>(cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronQueueWork<TWork, TResult>(this IWorkScheduler scheduler, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => scheduler.AddCronWork(CreateWork(provider => provider.EnqueueWork<TWork, TResult>(cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronAsyncQueueWork<TAsyncWork>(this IWorkScheduler scheduler, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => scheduler.AddCronWork(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork>(cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <param name="scheduler"> Work scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    public static void AddCronAsyncQueueWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => scheduler.AddCronWork(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, attemptsCount, priority).Ignore()), cronExpression, cancellation);
}

}
