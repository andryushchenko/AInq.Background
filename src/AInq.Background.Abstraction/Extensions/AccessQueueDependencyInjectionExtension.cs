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
using static AInq.Background.Tasks.AccessFactory;

namespace AInq.Background.Extensions;

/// <summary> <see cref="IAccessQueue{TResource}" /> and <see cref="IPriorityAccessQueue{T}" /> extensions to enqueue access action from DI </summary>
public static class AccessQueueDependencyInjectionExtension
{
#region BaseDI

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueAccess<TResource, TAccess>(this IAccessQueue<TResource> queue, CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(
            CreateAccess<TResource>((resource, provider) => provider.RequiredService<TAccess>().Access(resource, provider)),
            cancellation,
            attemptsCount);

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IAccessQueue<TResource> queue,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(
            CreateAccess<TResource, TResult>((resource, provider) => provider.RequiredService<TAccess>().Access(resource, provider)),
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IAccessQueue<TResource> queue, CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(
            CreateAsyncAccess<TResource>((resource, provider, cancel)
                => provider.RequiredService<TAsyncAccess>().AccessAsync(resource, provider, cancel)),
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IAccessQueue<TResource> queue,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(
            CreateAsyncAccess<TResource, TResult>((resource, provider, cancel)
                => provider.RequiredService<TAsyncAccess>().AccessAsync(resource, provider, cancel)),
            cancellation,
            attemptsCount);

#endregion

#region PriorityDI

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueAccess<TResource, TAccess>(this IPriorityAccessQueue<TResource> queue, int priority,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(
            CreateAccess<TResource>((resource, provider) => provider.RequiredService<TAccess>().Access(resource, provider)),
            priority,
            cancellation,
            attemptsCount);

    /// <summary> Enqueue access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IPriorityAccessQueue<TResource> queue, int priority,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(
            CreateAccess<TResource, TResult>((resource, provider) => provider.RequiredService<TAccess>().Access(resource, provider)),
            priority,
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IPriorityAccessQueue<TResource> queue, int priority,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(
            CreateAsyncAccess<TResource>((resource, provider, cancel)
                => provider.RequiredService<TAsyncAccess>().AccessAsync(resource, provider, cancel)),
            priority,
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="queue"> Access queue instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IPriorityAccessQueue<TResource> queue, int priority,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(
            CreateAsyncAccess<TResource, TResult>((resource, provider, cancel)
                => provider.RequiredService<TAsyncAccess>().AccessAsync(resource, provider, cancel)),
            priority,
            cancellation,
            attemptsCount);

#endregion
}
