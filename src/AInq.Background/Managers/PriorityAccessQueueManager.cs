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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.AccessFactory;
using static AInq.Background.Wrappers.AccessWrapperFactory;

namespace AInq.Background.Managers
{

internal sealed class PriorityAccessQueueManager<TResource> : AccessQueueManager<TResource>, IPriorityAccessQueue<TResource>, ITaskManager<TResource, int>
{
    private readonly int _maxPriority;
    private readonly IList<ConcurrentQueue<ITaskWrapper<TResource>>> _queues;

    int IPriorityAccessQueue<TResource>.MaxPriority => _maxPriority;

    bool ITaskManager<TResource, int>.HasTask => _queues.Any(queue => !queue.IsEmpty);

    Task ITaskManager<TResource, int>.WaitForTaskAsync(CancellationToken cancellation)
        => _queues.Any(queue => !queue.IsEmpty)
            ? Task.CompletedTask
            : NewAccessEvent.WaitAsync(cancellation);

    (ITaskWrapper<TResource>?, int) ITaskManager<TResource, int>.GetTask()
    {
        var pendingQueue = _queues.FirstOrDefault(queue => !queue.IsEmpty);
        return pendingQueue != null && pendingQueue.TryDequeue(out var task)
            ? (task, _queues.IndexOf(pendingQueue))
            : ((ITaskWrapper<TResource>?) null, -1);
    }

    void ITaskManager<TResource, int>.RevertTask(ITaskWrapper<TResource> task, int metadata)
    {
        _queues[FixPriority(metadata)].Enqueue(task);
        NewAccessEvent.Set();
    }

    internal PriorityAccessQueueManager(int maxPriority = 100, int maxAttempts = int.MaxValue) : base(maxAttempts)
    {
        _maxPriority = Math.Min(100, Math.Max(1, maxPriority));
        _queues = new ConcurrentQueue<ITaskWrapper<TResource>>[_maxPriority + 1];
        _queues[0] = Queue;
        for (var index = 1; index <= _maxPriority; index++)
            _queues[index] = new ConcurrentQueue<ITaskWrapper<TResource>>();
    }

    Task IPriorityAccessQueue<TResource>.EnqueueAccess(IAccess<TResource> access, int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task IPriorityAccessQueue<TResource>.EnqueueAccess<TAccess>(int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(CreateAccess<TResource>((resource, provider) => provider.GetRequiredService<TAccess>().Access(resource, provider)), FixAttempts(attemptsCount), cancellation);
        _queues[priority].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IPriorityAccessQueue<TResource>.EnqueueAccess<TResult>(IAccess<TResource, TResult> access, int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IPriorityAccessQueue<TResource>.EnqueueAccess<TAccess, TResult>(int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(CreateAccess<TResource, TResult>((resource, provider) => provider.GetRequiredService<TAccess>().Access(resource, provider)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task IPriorityAccessQueue<TResource>.EnqueueAsyncAccess(IAsyncAccess<TResource> access, int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task IPriorityAccessQueue<TResource>.EnqueueAsyncAccess<TAsyncAccess>(int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(CreateAccess<TResource>((resource, provider, token) => provider.GetRequiredService<TAsyncAccess>().AccessAsync(resource, provider, token)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IPriorityAccessQueue<TResource>.EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    Task<TResult> IPriorityAccessQueue<TResource>.EnqueueAsyncAccess<TAsyncAccess, TResult>(int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(CreateAccess<TResource, TResult>((resource, provider, token) => provider.GetRequiredService<TAsyncAccess>().AccessAsync(resource, provider, token)), FixAttempts(attemptsCount), cancellation);
        _queues[FixPriority(priority)].Enqueue(accessWrapper);
        NewAccessEvent.Set();
        return task;
    }

    private int FixPriority(int priority)
        => Math.Min(_maxPriority, Math.Max(0, priority));
}

}
