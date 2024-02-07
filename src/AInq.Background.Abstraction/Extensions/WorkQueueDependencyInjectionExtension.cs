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
using static AInq.Background.Tasks.InjectedWorkFactory;

namespace AInq.Background.Extensions;

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> extensions to enqueue work from DI </summary>
public static class WorkQueueDependencyInjectionExtension
{
#region BaseDI

    /// <summary> Enqueue background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    [PublicAPI]
    public static Task EnqueueWork<TWork>(this IWorkQueue queue, int attemptsCount = 1, CancellationToken cancellation = default)
        where TWork : IWork
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueWork(CreateInjectedWork<TWork>(), attemptsCount, cancellation);

    /// <summary> Enqueue background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IWorkQueue queue, int attemptsCount = 1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueWork(CreateInjectedWork<TWork, TResult>(), attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    [PublicAPI]
    public static Task EnqueueAsyncWork<TAsyncWork>(this IWorkQueue queue, int attemptsCount = 1, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncWork(CreateInjectedAsyncWork<TAsyncWork>(), attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IWorkQueue queue, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncWork(CreateInjectedAsyncWork<TAsyncWork, TResult>(), attemptsCount, cancellation);

#endregion

#region PriorityDI

    /// <summary> Enqueue background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    [PublicAPI]
    public static Task EnqueueWork<TWork>(this IPriorityWorkQueue queue, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TWork : IWork
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueWork(CreateInjectedWork<TWork>(), priority, attemptsCount, cancellation);

    /// <summary> Enqueue background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IPriorityWorkQueue queue, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueWork(CreateInjectedWork<TWork, TResult>(), priority, attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    [PublicAPI]
    public static Task EnqueueAsyncWork<TAsyncWork>(this IPriorityWorkQueue queue, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncWork(CreateInjectedAsyncWork<TAsyncWork>(), priority, attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work completion task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IPriorityWorkQueue queue, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncWork(CreateInjectedAsyncWork<TAsyncWork, TResult>(), priority, attemptsCount, cancellation);

#endregion
}
