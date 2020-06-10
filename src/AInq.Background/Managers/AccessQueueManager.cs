/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using AInq.Background.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.AccessFactory;
using static AInq.Background.Wrappers.AccessWrapperFactory;

namespace AInq.Background.Managers
{

internal class AccessQueueManager<TResource> : IAccessQueue<TResource>, ITaskManager<TResource, object?>
{
    protected readonly ConcurrentQueue<ITaskWrapper<TResource>> Queue = new ConcurrentQueue<ITaskWrapper<TResource>>();
    protected readonly AsyncAutoResetEvent NewAccessEvent = new AsyncAutoResetEvent(false);

    bool ITaskManager<TResource, object?>.HasTask => !Queue.IsEmpty;

    Task ITaskManager<TResource, object?>.WaitForTaskAsync(CancellationToken cancellation)
        => Queue.IsEmpty
            ? NewAccessEvent.WaitAsync(cancellation)
            : Task.CompletedTask;

    (ITaskWrapper<TResource>?, object?) ITaskManager<TResource, object?>.GetTask()
        => (Queue.TryDequeue(out var task)
                ? task
                : null, null);

    void ITaskManager<TResource, object?>.RevertTask(ITaskWrapper<TResource> task, object? metadata)
        => Queue.Enqueue(task);

    Task IAccessQueue<TResource>.EnqueueAccess(IAccess<TResource> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)),
            attemptsCount < 1
                ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                : attemptsCount,
            cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAccess<TAccess>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(CreateAccess<TResource>((resource, provider) => provider.GetRequiredService<TAccess>().Access(resource, provider)),
                attemptsCount < 1
                    ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                    : attemptsCount,
                cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAccess<TResult>(IAccess<TResource, TResult> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)),
            attemptsCount < 1
                ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                : attemptsCount,
            cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAccess<TAccess, TResult>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(CreateAccess<TResource, TResult>((resource, provider) => provider.GetRequiredService<TAccess>().Access(resource, provider)),
            attemptsCount < 1
                ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                : attemptsCount,
            cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAsyncAccess(IAsyncAccess<TResource> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)),
            attemptsCount < 1
                ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                : attemptsCount,
            cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAsyncAccess<TAsyncAccess>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(CreateAccess<TResource>((resource, provider, token) => provider.GetRequiredService<TAsyncAccess>().AccessAsync(resource, provider, token)),
            attemptsCount < 1
                ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                : attemptsCount,
            cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)),
            attemptsCount < 1
                ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                : attemptsCount,
            cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAsyncAccess<TAsyncAccess, TResult>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(CreateAccess<TResource, TResult>((resource, provider, token) => provider.GetRequiredService<TAsyncAccess>().AccessAsync(resource, provider, token)),
                attemptsCount < 1
                    ? throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, "Must be 1 or greater")
                    : attemptsCount,
                cancellation);
        Queue.Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }
}

}
