﻿// Copyright 2020-2022 Anton Andryushchenko
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
using static AInq.Background.Tasks.QueuedAccessFactory;

namespace AInq.Background.Interaction;

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> extensions to run access in background queue </summary>
/// <remarks> <see cref="IPriorityAccessQueue{TResource}" /> or <see cref="IAccessQueue{TResource}" /> service should be registered on host to run queued access </remarks>
public static class WorkQueueAccessQueueInteraction
{
#region QueueAccess

    /// <summary> Enqueue access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAccess<TResource>(this IWorkQueue queue, IAccess<TResource> access, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var work = CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

    /// <summary> Enqueue access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TResult>(this IWorkQueue queue, IAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var work = CreateQueuedAccess(access ?? throw new ArgumentNullException(nameof(access)), attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

    /// <summary> Enqueue asynchronous access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="asyncAccess"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource>(this IWorkQueue queue, IAsyncAccess<TResource> asyncAccess,
        CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var work = CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

    /// <summary> Enqueue asynchronous access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="asyncAccess"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TResult>(this IWorkQueue queue, IAsyncAccess<TResource, TResult> asyncAccess,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var work = CreateQueuedAsyncAccess(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

#endregion

#region QueueAccessDI

    /// <summary> Enqueue access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAccess<TResource, TAccess>(this IWorkQueue queue, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
    {
        var work = CreateQueuedInjectedAccess<TResource, TAccess>(attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

    /// <summary> Enqueue access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IWorkQueue queue, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
    {
        var work = CreateQueuedInjectedAccess<TResource, TAccess, TResult>(attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

    /// <summary> Enqueue asynchronous access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IWorkQueue queue, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
    {
        var work = CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

    /// <summary> Enqueue asynchronous access action into work queue with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TAsyncAccess"> Access action type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IWorkQueue queue, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
    {
        var work = CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(attemptsCount, priority);
        return (queue ?? throw new ArgumentNullException(nameof(queue))) is IPriorityWorkQueue priorityQueue
            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation)
            : queue.EnqueueAsyncWork(work, cancellation);
    }

#endregion
}
