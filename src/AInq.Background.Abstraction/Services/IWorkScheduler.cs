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

using AInq.Background.Tasks;
using System;
using System.Threading;

namespace AInq.Background.Services
{

/// <summary> Interface for work scheduler service </summary>
public interface IWorkScheduler
{
    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedWork(IWork work, TimeSpan delay, CancellationToken cancellation = default);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedAsyncWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default);

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default);

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedWork<TWork>(TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork;

    /// <summary> Add delayed work to scheduler </summary>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedWork<TWork, TResult>(TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork<TResult>;

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedAsyncWork<TAsyncWork>(TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork;

    /// <summary> Add delayed asynchronous work to scheduler </summary>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    void AddDelayedAsyncWork<TAsyncWork, TResult>(TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>;

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default);

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledWork<TWork>(DateTime time, CancellationToken cancellation = default)
        where TWork : IWork;

    /// <summary> Add scheduled work to scheduler </summary>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledWork<TWork, TResult>(DateTime time, CancellationToken cancellation = default)
        where TWork : IWork<TResult>;

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledAsyncWork<TAsyncWork>(DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork;

    /// <summary> Add scheduled asynchronous work to scheduler </summary>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    void AddScheduledAsyncWork<TAsyncWork, TResult>(DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>;

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronWork(IWork work, string cronExpression, CancellationToken cancellation = default);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation = default);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation = default);

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default);

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronWork<TWork>(string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork;

    /// <summary> Add CRON-scheduled work to scheduler </summary>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronWork<TWork, TResult>(string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork<TResult>;

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronAsyncWork<TAsyncWork>(string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork;

    /// <summary> Add CRON-scheduled asynchronous work to scheduler </summary>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    void AddCronAsyncWork<TAsyncWork, TResult>(string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>;
}

}
