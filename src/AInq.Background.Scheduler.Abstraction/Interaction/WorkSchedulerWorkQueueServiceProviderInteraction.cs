// Copyright 2020-2021 Anton Andryushchenko
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

using AInq.Background.Helpers;
using AInq.Background.Services;
using AInq.Background.Tasks;
using AInq.Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Interaction
{

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled work in registered background queue  </summary>
/// <remarks> <see cref="IWorkScheduler" /> service should be registered on host to schedule work </remarks>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkSchedulerWorkQueueServiceProviderInteraction
{
#region DelayedQueue

    /// <summary> Add delayed queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork(IWorkScheduler,IWork,TimeSpan,CancellationToken,int,int)" />
    public static Task AddScheduledQueueWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TResult}(IWorkScheduler, IWork{TResult}, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork(work, delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork(IWorkScheduler,IAsyncWork,TimeSpan,CancellationToken,int,int)" />
    public static Task AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork(work,
                                                                              delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork(work,
                                                                              delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region DelayedQueueDI

    /// <summary> Add delayed queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task AddScheduledQueueWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork<TWork>(delay, cancellation, attemptsCount, priority);

    /// <summary> Add delayed queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork,TResult}(IWorkScheduler,DateTime,CancellationToken,int,int)" />
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork<TWork, TResult>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork<TAsyncWork>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork,TResult}(IWorkScheduler,DateTime,CancellationToken,int,int)" />
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region ScheduledQueue

    /// <summary> Add scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork(IWorkScheduler, IWork, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledQueueWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TResult}(IWorkScheduler, IWork{TResult}, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork(IWorkScheduler, IAsyncWork, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork(work,
                                                                              time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork(work,
                                                                              time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region ScheduledQueueDI

    /// <summary> Add scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledQueueWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork<TWork>(time, cancellation, attemptsCount, priority);

    /// <summary> Add scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueWork<TWork, TResult>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork<TAsyncWork>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
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
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region CronQueue

    /// <summary> Add CRON-scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork(IWorkScheduler, IWork, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronQueueWork(this IServiceProvider provider, IWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueWork(work,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TResult}(IWorkScheduler, IWork{TResult}, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueWork(work,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork(IWorkScheduler, IAsyncWork, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueWork(work,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueWork(work,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region CronQueueDI

    /// <summary> Add CRON-scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TWork}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronQueueWork<TWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueWork<TWork>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TWork, TResult}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronQueueWork<TWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueWork<TWork, TResult>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TAsyncWork}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueWork<TAsyncWork>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueWork<TAsyncWork, TResult>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedScheduledQueue

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork(IWorkScheduler,IWork,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IServiceProvider provider, IWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork(work,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TResult}(IWorkScheduler,IWork{TResult},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork(work,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork(IWorkScheduler,IAsyncWork,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork(work,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork(work,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedScheduledQueueDI

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IServiceProvider provider, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork<TWork>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork<TWork, TResult>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork<TAsyncWork>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time</param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedDelayedQueue

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork(IWorkScheduler,IWork,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork(this IServiceProvider provider, IWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork(work,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TResult}(IWorkScheduler,IWork{TResult},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork(work,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork(IWorkScheduler,IAsyncWork,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork(work,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork(work,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedDelayedQueueDI

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(this IServiceProvider provider, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork<TWork>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueWork<TWork, TResult>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork<TAsyncWork>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous work to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <seealso
    ///     cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion
}

}
