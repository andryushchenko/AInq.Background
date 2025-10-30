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

namespace AInq.Background.Extensions;

/// <summary> <see cref="IWorkScheduler" /> extensions to schedule work with delayed start </summary>
public static class WorkSchedulerDelayExtension
{
    /// <param name="scheduler"> Work Scheduler instance </param>
    extension(IWorkScheduler scheduler)
    {
#region Delayed

        /// <summary> Add delayed work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="delay"> Work execution delay </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <returns> Work result task </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
        [PublicAPI]
        public Task AddScheduledWork(IWork work, TimeSpan delay, CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

        /// <summary> Add delayed work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="delay"> Work execution delay </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <typeparam name="TResult"> Work result type </typeparam>
        /// <returns> Work result task </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
        [PublicAPI]
        public Task<TResult> AddScheduledWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

        /// <summary> Add delayed asynchronous work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="delay"> Work execution delay </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <returns> Work result task </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
        [PublicAPI]
        public Task AddScheduledAsyncWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

        /// <summary> Add delayed asynchronous work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="delay"> Work execution delay </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <typeparam name="TResult"> Work result type </typeparam>
        /// <returns> Work result task </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00.000 </exception>
        [PublicAPI]
        public Task<TResult> AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddScheduledAsyncWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(delay > TimeSpan.Zero
                    ? delay
                    : throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater than 00:00:00.000")),
                cancellation);

#endregion

#region RepeatedDelayed

        /// <summary> Add repeated work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="startDelay"> Work first execution delay </param>
        /// <param name="repeatDelay"> Work repeat delay </param>
        /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <returns> Work result observable </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedWork(IWork work, TimeSpan startDelay, TimeSpan repeatDelay, int execCount = -1,
            CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(startDelay),
                repeatDelay,
                execCount,
                cancellation);

        /// <summary> Add repeated work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="startDelay"> Work first execution delay </param>
        /// <param name="repeatDelay"> Work repeat delay </param>
        /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <typeparam name="TResult"> Work result type </typeparam>
        /// <returns> Work result observable </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedWork<TResult>(IWork<TResult> work, TimeSpan startDelay, TimeSpan repeatDelay, int execCount = -1,
            CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(startDelay),
                repeatDelay,
                execCount,
                cancellation);

        /// <summary> Add repeated asynchronous work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="startDelay"> Work first execution delay </param>
        /// <param name="repeatDelay"> Work repeat delay </param>
        /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <returns> Work result observable </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedAsyncWork(IAsyncWork work, TimeSpan startDelay, TimeSpan repeatDelay, int execCount = -1,
            CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(startDelay),
                repeatDelay,
                execCount,
                cancellation);

        /// <summary> Add repeated asynchronous work to scheduler </summary>
        /// <param name="work"> Work instance </param>
        /// <param name="startDelay"> Work first execution delay </param>
        /// <param name="repeatDelay"> Work repeat delay </param>
        /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
        /// <param name="cancellation"> Work cancellation token </param>
        /// <typeparam name="TResult"> Work result type </typeparam>
        /// <returns> Work result observable </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan startDelay, TimeSpan repeatDelay,
            int execCount = -1, CancellationToken cancellation = default)
            => (scheduler ?? throw new ArgumentNullException(nameof(scheduler))).AddRepeatedAsyncWork(
                work ?? throw new ArgumentNullException(nameof(work)),
                DateTime.Now.Add(startDelay),
                repeatDelay,
                execCount,
                cancellation);

#endregion
    }
}
