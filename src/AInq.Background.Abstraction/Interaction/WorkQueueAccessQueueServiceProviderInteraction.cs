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

using AInq.Background.Helpers;
using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Interaction
{

/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
/// <remarks>
///     <see cref="IPriorityAccessQueue{TResource}" /> or <see cref="IAccessQueue{TResource}" /> service should be registered on host to run queued
///     access
/// </remarks>
public static class WorkQueueAccessQueueServiceProviderInteraction
{
#region QueueAccess

    /// <summary> Enqueue access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource}(IWorkQueue,IAccess{TResource},CancellationToken,int,int)" />
    public static Task EnqueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAccess(access, cancellation, attemptsCount, priority);

    /// <summary> Enqueue access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource, TResult}(IWorkQueue,IAccess{TResource, TResult},CancellationToken,int,int)" />
    public static Task<TResult> EnqueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAccess(access, cancellation, attemptsCount, priority);

    /// <summary> Enqueue asynchronous access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource}(IWorkQueue,IAsyncAccess{TResource},CancellationToken,int,int)" />
    public static Task EnqueueAsyncAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAsyncAccess(access, cancellation, attemptsCount, priority);

    /// <summary> Enqueue asynchronous access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso
    ///     cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource, TResult}(IWorkQueue,IAsyncAccess{TResource, TResult},CancellationToken,int,int)" />
    public static Task<TResult> EnqueueAsyncAccess<TResource, TResult>(this IServiceProvider provider, IAsyncAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAsyncAccess(access, cancellation, attemptsCount, priority);

#endregion

#region QueueAccessDI

    /// <summary> Enqueue access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource, TAccess}(IWorkQueue,CancellationToken,int,int)" />
    public static Task EnqueueAccess<TResource, TAccess>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAccess<TResource, TAccess>(cancellation, attemptsCount, priority);

    /// <summary> Enqueue access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource, TAccess, TResult}(IWorkQueue,CancellationToken,int,int)" />
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAccess<TResource, TAccess, TResult>(cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Enqueue asynchronous access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource, TAsyncAccess}(IWorkQueue,CancellationToken,int,int)" />
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAsyncAccess<TResource, TAsyncAccess>(cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Enqueue asynchronous access action into registered work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource, TAsyncAccess, TResult}(IWorkQueue,CancellationToken,int,int)" />
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>()
                                                                          .EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion
}

}
