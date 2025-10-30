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
    /// <param name="queue"> Access queue instance </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    extension<TResource>(IAccessQueue<TResource> queue)
        where TResource : notnull
    {
        /// <summary> Enqueue access action </summary>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAccess"> Access action type </typeparam>
        /// <returns> Access action completion task </returns>
        [PublicAPI]
        public Task EnqueueAccess<TAccess>(int attemptsCount = 1, CancellationToken cancellation = default)
            where TAccess : IAccess<TResource>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(CreateInjectedAccess<TResource, TAccess>(),
                attemptsCount,
                cancellation);

        /// <summary> Enqueue access action </summary>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAccess"> Access action type </typeparam>
        /// <typeparam name="TResult"> Access action result type </typeparam>
        /// <returns> Access action result task </returns>
        [PublicAPI]
        public Task<TResult> EnqueueAccess<TAccess, TResult>(int attemptsCount = 1, CancellationToken cancellation = default)
            where TAccess : IAccess<TResource, TResult>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(CreateInjectedAccess<TResource, TAccess, TResult>(),
                attemptsCount,
                cancellation);

        /// <summary> Enqueue asynchronous access action </summary>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
        /// <returns> Access action completion task </returns>
        [PublicAPI]
        public Task EnqueueAsyncAccess<TAsyncAccess>(int attemptsCount = 1, CancellationToken cancellation = default)
            where TAsyncAccess : IAsyncAccess<TResource>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(CreateInjectedAsyncAccess<TResource, TAsyncAccess>(),
                attemptsCount,
                cancellation);

        /// <summary> Enqueue asynchronous access action </summary>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
        /// <typeparam name="TResult"> Access action result type </typeparam>
        /// <returns> Access action result task </returns>
        [PublicAPI]
        public Task<TResult> EnqueueAsyncAccess<TAsyncAccess, TResult>(int attemptsCount = 1, CancellationToken cancellation = default)
            where TAsyncAccess : IAsyncAccess<TResource, TResult>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(
                CreateInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(),
                attemptsCount,
                cancellation);
    }

    /// <param name="queue"> Access queue instance </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    extension<TResource>(IPriorityAccessQueue<TResource> queue)
        where TResource : notnull
    {
        /// <summary> Enqueue access action </summary>
        /// <param name="priority"> Access action priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAccess"> Access action type </typeparam>
        /// <returns> Access action completion task </returns>
        [PublicAPI]
        public Task EnqueueAccess<TAccess>(int priority, int attemptsCount = 1, CancellationToken cancellation = default)
            where TAccess : IAccess<TResource>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(CreateInjectedAccess<TResource, TAccess>(),
                priority,
                attemptsCount,
                cancellation);

        /// <summary> Enqueue access action </summary>
        /// <param name="priority"> Access action priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAccess"> Access action type </typeparam>
        /// <typeparam name="TResult"> Access action result type </typeparam>
        /// <returns> Access action result task </returns>
        [PublicAPI]
        public Task<TResult> EnqueueAccess<TAccess, TResult>(int priority, int attemptsCount = 1, CancellationToken cancellation = default)
            where TAccess : IAccess<TResource, TResult>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAccess(CreateInjectedAccess<TResource, TAccess, TResult>(),
                priority,
                attemptsCount,
                cancellation);

        /// <summary> Enqueue asynchronous access action </summary>
        /// <param name="priority"> Access action priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
        /// <returns> Access action completion task </returns>
        [PublicAPI]
        public Task EnqueueAsyncAccess<TAsyncAccess>(int priority, int attemptsCount = 1, CancellationToken cancellation = default)
            where TAsyncAccess : IAsyncAccess<TResource>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(CreateInjectedAsyncAccess<TResource, TAsyncAccess>(),
                priority,
                attemptsCount,
                cancellation);

        /// <summary> Enqueue asynchronous access action </summary>
        /// <param name="priority"> Access action priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="cancellation"> Access cancellation token </param>
        /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
        /// <typeparam name="TResult"> Access action result type </typeparam>
        /// <returns> Access action result task </returns>
        [PublicAPI]
        public Task<TResult> EnqueueAsyncAccess<TAsyncAccess, TResult>(int priority, int attemptsCount = 1, CancellationToken cancellation = default)
            where TAsyncAccess : IAsyncAccess<TResource, TResult>
            => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncAccess(
                CreateInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(),
                priority,
                attemptsCount,
                cancellation);
    }
}
