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

using AInq.Background.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.WorkFactory;
using static AInq.Background.Wrappers.WorkWrapperFactory;

namespace AInq.Background.Managers
{

internal class WorkQueueManager : IWorkQueue, ITaskManager<object?, object?>
{
    protected readonly ConcurrentQueue<ITaskWrapper<object?>> Queue = new ConcurrentQueue<ITaskWrapper<object?>>();
    protected readonly AsyncAutoResetEvent NewWorkEvent = new AsyncAutoResetEvent(false);
    private readonly int _maxAttempts;

    internal WorkQueueManager(int maxAttempts = int.MaxValue)
    {
        _maxAttempts = Math.Max(maxAttempts, 1);
    }

    int IWorkQueue.MaxAttempts => _maxAttempts;
    bool ITaskManager<object?, object?>.HasTask => !Queue.IsEmpty;

    Task ITaskManager<object?, object?>.WaitForTaskAsync(CancellationToken cancellation)
        => Queue.IsEmpty
            ? NewWorkEvent.WaitAsync(cancellation)
            : Task.CompletedTask;

    (ITaskWrapper<object?>?, object?) ITaskManager<object?, object?>.GetTask()
    {
        while (true)
        {
            if (!Queue.TryDequeue(out var task))
                return (null, null);
            if (!task.IsCanceled)
                return (task, null);
        }
    }

    void ITaskManager<object?, object?>.RevertTask(ITaskWrapper<object?> task, object? metadata)
        => Queue.Enqueue(task);

    Task IWorkQueue.EnqueueWork(IWork work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task IWorkQueue.EnqueueWork<TWork>(CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueWork<TWork, TResult>(CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task IWorkQueue.EnqueueAsyncWork(IAsyncWork work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task IWorkQueue.EnqueueAsyncWork<TWork>(CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(CreateAsyncWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueAsyncWork<TWork, TResult>(CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(CreateAsyncWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), FixAttempts(attemptsCount), cancellation);
        Queue.Enqueue(workWrapper);
        NewWorkEvent.Set();
        return task;
    }

    protected int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}

}
