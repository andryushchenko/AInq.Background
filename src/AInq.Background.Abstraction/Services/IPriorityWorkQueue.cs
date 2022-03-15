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

using AInq.Background.Tasks;

namespace AInq.Background.Services;

/// <summary> Interface for background work queue with prioritization </summary>
public interface IPriorityWorkQueue : IWorkQueue
{
    /// <summary> Max allowed work priority </summary>
    [PublicAPI]
    int MaxPriority { get; }

    /// <summary> Enqueue background work with given <paramref name="priority" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    Task EnqueueWork(IWork work, int priority, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue background work with given <paramref name="priority" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    Task<TResult> EnqueueWork<TResult>(IWork<TResult> work, int priority, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue asynchronous background work with given <paramref name="priority" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    Task EnqueueAsyncWork(IAsyncWork work, int priority, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue asynchronous background work with given <paramref name="priority" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    Task<TResult> EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, int priority, CancellationToken cancellation = default, int attemptsCount = 1);
}
