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
using AInq.Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Extensions
{

/// <summary> <see cref="IWorkScheduler" /> extensions to schedule work with delayed start </summary>
public static class WorkSchedulerDelayExtension
{
    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task AddScheduledWork(this IWorkScheduler scheduler, IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledWork(work, DateTime.Now.Add(delay), cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task<TResult> AddScheduledWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledWork(work, DateTime.Now.Add(delay), cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task AddScheduledAsyncWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(work, DateTime.Now.Add(delay), cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00.000 </exception>
    public static Task<TResult> AddScheduledAsyncWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(work, DateTime.Now.Add(delay), cancellation);

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedWork(this IWorkScheduler scheduler, IWork work, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedWork(work,
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<Try<TResult>> AddRepeatedWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedWork(work,
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(work,
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="scheduler" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(work,
            DateTime.Now.Add(startDelay),
            repeatDelay,
            cancellation,
            execCount);
}

}
