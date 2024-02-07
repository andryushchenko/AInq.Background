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
using static AInq.Background.Tasks.InjectedAccessFactory;

namespace AInq.Background.Extensions;

/// <summary> <see cref="IAccessQueue{TResource}" /> and <see cref="IPriorityAccessQueue{T}" /> extensions to enqueue access action from DI </summary>
public static class AccessQueueDependencyInjectionExtension
{
#region BaseDI

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAccess<TResource, TAccess>(this IAccessQueue<TResource> queue, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAccess(CreateInjectedAccess<TResource, TAccess>(), attemptsCount, cancellation);

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IAccessQueue<TResource> queue, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAccess(CreateInjectedAccess<TResource, TAccess, TResult>(), attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IAccessQueue<TResource> queue, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncAccess(CreateInjectedAsyncAccess<TResource, TAsyncAccess>(), attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IAccessQueue<TResource> queue, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncAccess(CreateInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(), attemptsCount, cancellation);

#endregion

#region PriorityDI

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAccess<TResource, TAccess>(this IPriorityAccessQueue<TResource> queue, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAccess(CreateInjectedAccess<TResource, TAccess>(), priority, attemptsCount, cancellation);

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IPriorityAccessQueue<TResource> queue, int priority,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAccess(CreateInjectedAccess<TResource, TAccess, TResult>(), priority, attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IPriorityAccessQueue<TResource> queue, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncAccess(CreateInjectedAsyncAccess<TResource, TAsyncAccess>(), priority, attemptsCount, cancellation);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IPriorityAccessQueue<TResource> queue, int priority,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .EnqueueAsyncAccess(CreateInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(), priority, attemptsCount, cancellation);

#endregion
}
