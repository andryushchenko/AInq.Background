// Copyright 2020-2022 Anton Andryushchenko
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

namespace AInq.Background.Services;

/// <summary> Interface for work scheduler service </summary>
public interface IWorkScheduler
{
#region Scheduled

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    Task AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    Task<TResult> AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    Task AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    Task<TResult> AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default);

#endregion

#region Cron

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Maybe<Exception>> AddCronWork(IWork work, string cronExpression, CancellationToken cancellation = default, int execCount = -1);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Try<TResult>> AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation = default,
        int execCount = -1);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Maybe<Exception>> AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation = default,
        int execCount = -1);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cronExpression" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Try<TResult>> AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default,
        int execCount = -1);

#endregion

#region RepeatedScheduled

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Maybe<Exception>> AddRepeatedWork(IWork work, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int execCount = -1);

    /// <summary> Add repeated work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Try<TResult>> AddRepeatedWork<TResult>(IWork<TResult> work, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Maybe<Exception>> AddRepeatedAsyncWork(IAsyncWork work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1);

    /// <summary> Add repeated asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00.000 or <paramref name="execCount" /> is 0 or less than -1 </exception>
    IObservable<Try<TResult>> AddRepeatedAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1);

#endregion
}
