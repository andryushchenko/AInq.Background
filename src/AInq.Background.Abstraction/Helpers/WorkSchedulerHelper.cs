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

using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Helpers
{

/// <summary> Helper class for <see cref="IWorkScheduler" /> </summary>
public static class WorkSchedulerHelper
{
    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork(IWork, TimeSpan, CancellationToken)" />
    public static Task AddDelayedWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork(work, delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TResult}(IWork{TResult}, TimeSpan, CancellationToken)" />
    public static Task<TResult> AddDelayedWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork(work, delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedAsyncWork(IAsyncWork, TimeSpan, CancellationToken)" />
    public static Task AddDelayedAsyncWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork(work, delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedAsyncWork{TResult}(IAsyncWork{TResult}, TimeSpan, CancellationToken)" />
    public static Task<TResult> AddDelayedAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork(work, delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TWork}(TimeSpan, CancellationToken)" />
    public static Task AddDelayedWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork<TWork>(delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TWork, TResult}(TimeSpan, CancellationToken)" />
    public static Task<TResult> AddDelayedWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork<TWork, TResult>(delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedAsyncWork{TAsyncWork}(TimeSpan, CancellationToken)" />
    public static Task AddDelayedAsyncWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork<TAsyncWork>(delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TAsyncWork, TResult}(TimeSpan, CancellationToken)" />
    public static Task<TResult> AddDelayedAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork<TAsyncWork, TResult>(delay, cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedQueueWork(IWorkScheduler, IWork, TimeSpan, CancellationToken, int, int)" />
    public static Task AddDelayedQueueWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedQueueWork{TResult}(IWorkScheduler, IWork{TResult}, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddDelayedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedAsyncQueueWork(IWorkScheduler, IAsyncWork, TimeSpan, CancellationToken, int, int)" />
    public static Task AddDelayedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerQueueExtension.AddDelayedAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddDelayedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedQueueWork{TWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task AddDelayedQueueWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork<TWork>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedQueueWork{TWork, TResult}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddDelayedQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork<TWork, TResult>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedAsyncQueueWork{TAsyncWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task AddDelayedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork<TAsyncWork>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddDelayedAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddDelayedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork<TAsyncWork, TResult>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork(IWork, DateTime, CancellationToken)" />
    public static Task AddScheduledWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork(work, time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TResult}(IWork{TResult}, DateTime, CancellationToken)" />
    public static Task<TResult> AddScheduledWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork(work, time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork(IAsyncWork, DateTime, CancellationToken)" />
    public static Task AddScheduledAsyncWork(this IServiceProvider provider, IAsyncWork work, DateTime time, CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork(work, time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TResult}(IAsyncWork{TResult}, DateTime, CancellationToken)" />
    public static Task<TResult> AddScheduledAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork(work, time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TWork}(DateTime, CancellationToken)" />
    public static Task AddScheduledWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork<TWork>(time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TWork, TResult}(DateTime, CancellationToken)" />
    public static Task<TResult> AddScheduledWork<TWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork<TWork, TResult>(time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TAsyncWork}(DateTime, CancellationToken)" />
    public static Task AddScheduledAsyncWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork<TAsyncWork>(time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TAsyncWork, TResult}(DateTime, CancellationToken)" />
    public static Task<TResult> AddScheduledAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork<TAsyncWork, TResult>(time, cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledQueueWork(IWorkScheduler, IWork, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledQueueWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledQueueWork{TResult}(IWorkScheduler, IWork{TResult}, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledAsyncQueueWork(IWorkScheduler, IAsyncWork, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerQueueExtension.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledQueueWork{TWork}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledQueueWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork<TWork>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledQueueWork{TWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork<TWork, TResult>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork<TAsyncWork>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddScheduledAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(time, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork(IWork, string, CancellationToken)" />
    public static IObservable<bool> AddCronWork(this IServiceProvider provider, IWork work, string cronExpression, CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TResult}(IWork{TResult}, string, CancellationToken)" />
    public static IObservable<TResult> AddCronWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork(IAsyncWork, string, CancellationToken)" />
    public static IObservable<bool> AddCronAsyncWork(this IServiceProvider provider, IAsyncWork work, string cronExpression,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TResult}(IAsyncWork{TResult}, string, CancellationToken)" />
    public static IObservable<TResult> AddCronAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TWork}(string, CancellationToken)" />
    public static IObservable<bool> AddCronWork<TWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork<TWork>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TWork, TResult}(string, CancellationToken)" />
    public static IObservable<TResult> AddCronWork<TWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork<TWork, TResult>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TAsyncWork}(string, CancellationToken)" />
    public static IObservable<bool> AddCronAsyncWork<TAsyncWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork<TAsyncWork>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TAsyncWork, TResult}(string, CancellationToken)" />
    public static IObservable<TResult> AddCronAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork<TAsyncWork, TResult>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronQueueWork(IWorkScheduler, IWork, string, CancellationToken, int, int)" />
    public static IObservable<bool> AddCronQueueWork(this IServiceProvider provider, IWork work, string cronExpression, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronQueueWork{TResult}(IWorkScheduler, IWork{TResult}, string, CancellationToken, int, int)" />
    public static IObservable<TResult> AddCronQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronAsyncQueueWork(IWorkScheduler, IAsyncWork, string, CancellationToken, int, int)" />
    public static IObservable<bool> AddCronAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, string, CancellationToken, int, int)" />
    public static IObservable<TResult> AddCronAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronQueueWork{TWork}(IWorkScheduler, string, CancellationToken, int, int)" />
    public static IObservable<bool> AddCronQueueWork<TWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork<TWork>(cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronQueueWork{TWork, TResult}(IWorkScheduler, string, CancellationToken, int, int)" />
    public static IObservable<TResult> AddCronQueueWork<TWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork<TWork, TResult>(cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronAsyncQueueWork{TAsyncWork}(IWorkScheduler, string, CancellationToken, int, int)" />
    public static IObservable<bool> AddCronAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork<TAsyncWork>(cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueExtension.AddCronAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, string, CancellationToken, int, int)" />
    public static IObservable<TResult> AddCronAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkScheduler)) as IWorkScheduler
            ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork<TAsyncWork, TResult>(cronExpression, cancellation, attemptsCount, priority);
}

}
