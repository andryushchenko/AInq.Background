﻿// Copyright 2020-2021 Anton Andryushchenko
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

using AInq.Background.Extensions;
using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Helpers
{

/// <summary> Helper class for <see cref="IAccessQueue{TResource}" /> and <see cref="IPriorityAccessQueue{TResource}" /> </summary>
/// <remarks>
///     <see cref="IAccessQueue{TResource}" /> or <see cref="IPriorityAccessQueue{TResource}" /> service should be registered on host to run queued
///     access action
/// </remarks>
public static class AccessQueueServiceProviderHelper
{
#region Access

    /// <summary> Enqueue access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityAccessQueue{TResource}.EnqueueAccess(IAccess{TResource}, int, CancellationToken, int)" />
    public static Task EnqueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAccess(access, priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAccess(access, cancellation, attemptsCount);

    /// <summary> Enqueue access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityAccessQueue{TResource}.EnqueueAccess{TResult}(IAccess{TResource, TResult}, int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAccess(access, priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAccess(access, cancellation, attemptsCount);

    /// <summary> Enqueue asynchronous access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityAccessQueue{TResource}.EnqueueAsyncAccess(IAsyncAccess{TResource}, int, CancellationToken, int)" />
    public static Task EnqueueAsyncAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAsyncAccess(access, cancellation, attemptsCount);

    /// <summary> Enqueue asynchronous access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityAccessQueue{TResource}.EnqueueAsyncAccess{TResult}(IAsyncAccess{TResource, TResult}, int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAsyncAccess<TResource, TResult>(this IServiceProvider provider, IAsyncAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAsyncAccess(access, cancellation, attemptsCount);

#endregion

#region AccessDI

    /// <summary> Enqueue access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="AccessQueueDependencyInjectionExtension.EnqueueAccess{TResource,TAccess}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    public static Task EnqueueAccess<TResource, TAccess>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAccess<TResource, TAccess>(priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAccess<TResource, TAccess>(cancellation, attemptsCount);

    /// <summary> Enqueue access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso
    ///     cref="AccessQueueDependencyInjectionExtension.EnqueueAccess{TResource,TAccess, TResult}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAccess<TResource, TAccess, TResult>(priority,
                                                                              cancellation,
                                                                              attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAccess<TResource, TAccess, TResult>(cancellation, attemptsCount);

    /// <summary> Enqueue asynchronous access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso
    ///     cref="AccessQueueDependencyInjectionExtension.EnqueueAsyncAccess{TResource,TAsyncAccess}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAsyncAccess<TResource, TAsyncAccess>(priority,
                                                                              cancellation,
                                                                              attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAsyncAccess<TResource, TAsyncAccess>(cancellation, attemptsCount);

    /// <summary> Enqueue asynchronous access action into registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access action priority </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso
    ///     cref="AccessQueueDependencyInjectionExtension.EnqueueAsyncAccess{TResource,TAsyncAccess, TResult}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).Service<IPriorityAccessQueue<TResource>>()
                                                                          ?.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(priority,
                                                                              cancellation,
                                                                              attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>()
                      .EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancellation, attemptsCount);

#endregion
}

}
