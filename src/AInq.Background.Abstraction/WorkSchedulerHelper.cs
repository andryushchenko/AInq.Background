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

namespace AInq.Background
{

/// <summary> Helper class for <see cref="IWorkScheduler"/> </summary>
public static class WorkSchedulerHelper
{
    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork(IWork, TimeSpan, CancellationToken)"/>
    public static void AddDelayedWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork(work, delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TResult}(IWork{TResult}, TimeSpan, CancellationToken)"/>
    public static void AddDelayedWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork(work, delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedAsyncWork(IAsyncWork, TimeSpan, CancellationToken)"/>
    public static void AddDelayedAsyncWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork(work, delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedAsyncWork{TResult}(IAsyncWork{TResult}, TimeSpan, CancellationToken)"/>
    public static void AddDelayedAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork(work, delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TWork}(TimeSpan, CancellationToken)"/>
    public static void AddDelayedWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork<TWork>(delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TWork, TResult}(TimeSpan, CancellationToken)"/>
    public static void AddDelayedWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork<TWork, TResult>(delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedAsyncWork{TAsyncWork}(TimeSpan, CancellationToken)"/>
    public static void AddDelayedAsyncWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork<TAsyncWork>(delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddDelayedWork{TAsyncWork, TResult}(TimeSpan, CancellationToken)"/>
    public static void AddDelayedAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork<TAsyncWork, TResult>(delay, cancellation);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedQueueWork(IWorkScheduler, IWork, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedQueueWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedQueueWork{TResult}(IWorkScheduler, IWork{TResult}, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedAsyncQueueWork(IWorkScheduler, IAsyncWork, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedQueueWork{TWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedQueueWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork<TWork>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedQueueWork{TWork, TResult}(IWorkScheduler, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork<TWork, TResult>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedAsyncQueueWork{TAsyncWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork<TAsyncWork>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay"/> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddDelayedAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, TimeSpan, CancellationToken, int, int)"/>
    public static void AddDelayedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork<TAsyncWork, TResult>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork(IWork, DateTime, CancellationToken)"/>
    public static void AddScheduledWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork(work, time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TResult}(IWork{TResult}, DateTime, CancellationToken)"/>
    public static void AddScheduledWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork(work, time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork(IAsyncWork, DateTime, CancellationToken)"/>
    public static void AddScheduledAsyncWork(this IServiceProvider provider, IAsyncWork work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork(work, time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TResult}(IAsyncWork{TResult}, DateTime, CancellationToken)"/>
    public static void AddScheduledAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork(work, time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TWork}(DateTime, CancellationToken)"/>
    public static void AddScheduledWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork<TWork>(time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TWork, TResult}(DateTime, CancellationToken)"/>
    public static void AddScheduledWork<TWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork<TWork, TResult>(time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TAsyncWork}(DateTime, CancellationToken)"/>
    public static void AddScheduledAsyncWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork<TAsyncWork>(time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TAsyncWork, TResult}(DateTime, CancellationToken)"/>
    public static void AddScheduledAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork<TAsyncWork, TResult>(time, cancellation);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledQueueWork(IWorkScheduler, IWork, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledQueueWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledQueueWork{TResult}(IWorkScheduler, IWork{TResult}, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledAsyncQueueWork(IWorkScheduler, IAsyncWork, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledQueueWork{TWork}(IWorkScheduler, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledQueueWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork<TWork>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledQueueWork{TWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork<TWork, TResult>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork<TAsyncWork>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time"/> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddScheduledAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)"/>
    public static void AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(time, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork(IWork, string, CancellationToken)"/>
    public static void AddCronWork(this IServiceProvider provider, IWork work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TResult}(IWork{TResult}, string, CancellationToken)"/>
    public static void AddCronWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork(IAsyncWork, string, CancellationToken)"/>
    public static void AddCronAsyncWork(this IServiceProvider provider, IAsyncWork work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TResult}(IAsyncWork{TResult}, string, CancellationToken)"/>
    public static void AddCronAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork(work, cronExpression, cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TWork}(string, CancellationToken)"/>
    public static void AddCronWork<TWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork<TWork>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TWork, TResult}(string, CancellationToken)"/>
    public static void AddCronWork<TWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork<TWork, TResult>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TAsyncWork}(string, CancellationToken)"/>
    public static void AddCronAsyncWork<TAsyncWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork<TAsyncWork>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TAsyncWork, TResult}(string, CancellationToken)"/>
    public static void AddCronAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork<TAsyncWork, TResult>(cronExpression, cancellation);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronQueueWork(IWorkScheduler, IWork, string, CancellationToken, int, int)"/>
    public static void AddCronQueueWork(this IServiceProvider provider, IWork work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronQueueWork{TResult}(IWorkScheduler, IWork{TResult}, string, CancellationToken, int, int)"/>
    public static void AddCronQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronAsyncQueueWork(IWorkScheduler, IAsyncWork, string, CancellationToken, int, int)"/>
    public static void AddCronAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, string, CancellationToken, int, int)"/>
    public static void AddCronAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronQueueWork{TWork}(IWorkScheduler, string, CancellationToken, int, int)"/>
    public static void AddCronQueueWork<TWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork<TWork>(cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronQueueWork{TWork, TResult}(IWorkScheduler, string, CancellationToken, int, int)"/>
    public static void AddCronQueueWork<TWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork<TWork, TResult>(cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronAsyncQueueWork{TAsyncWork}(IWorkScheduler, string, CancellationToken, int, int)"/>
    public static void AddCronAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork<TAsyncWork>(cronExpression, cancellation, attemptsCount, priority);

    /// <summary> Add CRON-scheduled queued asynchronous work to scheduler with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression"/> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerQueueHelper.AddCronAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, string, CancellationToken, int, int)"/>
    public static void AddCronAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork<TAsyncWork, TResult>(cronExpression, cancellation, attemptsCount, priority);
}

}
