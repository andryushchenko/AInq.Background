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

namespace AInq.Background.Extensions;

/// <summary> <see cref="IWorkScheduler" /> extensions to schedule work with delayed start </summary>
public static class WorkSchedulerDelayExtension
{
#region Delayed

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledWork(this IWorkScheduler scheduler, IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledWork(work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledWork(work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task AddScheduledAsyncWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddScheduledAsyncWork(work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

#endregion

#region RepeatedDelayed

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="scheduler"> Work Scheduler instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork(this IWorkScheduler scheduler, IWork work, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedWork(work ?? throw new ArgumentNullException(nameof(work)),
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TResult>(this IWorkScheduler scheduler, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedWork(work ?? throw new ArgumentNullException(nameof(work)),
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork(this IWorkScheduler scheduler, IAsyncWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(work ?? throw new ArgumentNullException(nameof(work)),
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TResult>(this IWorkScheduler scheduler, IAsyncWork<TResult> work,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (scheduler ?? throw new ArgumentNullException(nameof(scheduler)))
            .AddRepeatedAsyncWork(work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(startDelay),
                repeatDelay,
                cancellation,
                execCount);

#endregion
}
