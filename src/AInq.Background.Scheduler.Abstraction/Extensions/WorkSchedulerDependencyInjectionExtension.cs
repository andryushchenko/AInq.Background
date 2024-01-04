// Copyright 2020-2024 Anton Andryushchenko
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

using static AInq.Background.Tasks.InjectedWorkFactory;

namespace AInq.Background.Extensions;

/// <summary> <see cref="IWorkScheduler" /> extensions to schedule work from DI </summary>
public static class WorkSchedulerDependencyInjectionExtension
{
#region ScheduledDI

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledWork<TWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledWork(CreateInjectedWork<TWork>(), time, cancellation);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledWork(CreateInjectedWork<TWork, TResult>(), time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateInjectedAsyncWork<TAsyncWork>(), time, cancellation);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime time,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateInjectedAsyncWork<TAsyncWork, TResult>(), time, cancellation);

#endregion

#region DelayedDI

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledWork<TWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledWork(CreateInjectedWork<TWork>(), delay, cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledWork(CreateInjectedWork<TWork, TResult>(), delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateInjectedAsyncWork<TAsyncWork>(), delay, cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan delay,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(CreateInjectedAsyncWork<TAsyncWork, TResult>(), delay, cancellation);

#endregion

#region CronDI

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronWork<TWork>(this IWorkScheduler scheduler, string cronExpression, int execCount = -1,
        CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronWork(CreateInjectedWork<TWork>(),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronWork<TWork, TResult>(this IWorkScheduler scheduler, string cronExpression, int execCount = -1,
        CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronWork(CreateInjectedWork<TWork, TResult>(),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncWork<TAsyncWork>(this IWorkScheduler scheduler, string cronExpression, int execCount = -1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(CreateInjectedAsyncWork<TAsyncWork>(),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, string cronExpression,
        int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddCronAsyncWork(CreateInjectedAsyncWork<TAsyncWork, TResult>(),
            cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
            execCount,
            cancellation);

#endregion

#region RepeatedScheduledDI

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork<TWork>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedWork(CreateInjectedWork<TWork>(), starTime, repeatDelay, execCount, cancellation);

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TWork, TResult>(this IWorkScheduler scheduler, DateTime starTime, TimeSpan repeatDelay,
        int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedWork(CreateInjectedWork<TWork, TResult>(), starTime, repeatDelay, execCount, cancellation);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork<TAsyncWork>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(CreateInjectedAsyncWork<TAsyncWork>(), starTime, repeatDelay, execCount, cancellation);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, DateTime starTime,
        TimeSpan repeatDelay, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(CreateInjectedAsyncWork<TAsyncWork, TResult>(), starTime, repeatDelay, execCount, cancellation);

#endregion

#region RepeatedDelayedDI

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork<TWork>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedWork<TWork>(DateTime.Now.Add(startDelay), repeatDelay, execCount, cancellation);

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay, TimeSpan repeatDelay,
        int execCount = -1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedWork<TWork, TResult>(DateTime.Now.Add(startDelay), repeatDelay, execCount, cancellation);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork<TAsyncWork>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork<TAsyncWork>(DateTime.Now.Add(startDelay), repeatDelay, execCount, cancellation);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TAsyncWork, TResult>(this IWorkScheduler scheduler, TimeSpan startDelay,
        TimeSpan repeatDelay, int execCount = -1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork<TAsyncWork, TResult>(DateTime.Now.Add(startDelay), repeatDelay, execCount, cancellation);

#endregion
}
