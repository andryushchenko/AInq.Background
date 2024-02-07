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

using static AInq.Background.Wrappers.WorkWrapperFactory;

namespace AInq.Background.Managers;

/// <summary> Background work queue manager with numeric prioritization </summary>
public sealed class PriorityWorkQueueManager : PriorityTaskManager<object?>, IPriorityWorkQueue
{
    private readonly int _maxAttempts;

    /// <param name="maxPriority"> Max allowed work priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public PriorityWorkQueueManager(int maxPriority = 100, int maxAttempts = int.MaxValue) : base(maxPriority)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IWorkQueue.MaxAttempts => _maxAttempts;
    int IPriorityWorkQueue.MaxPriority => MaxPriority;

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));

#region Base

    Task IWorkQueue.EnqueueWork(IWork work, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, 0);
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, 0);
        return task;
    }

    Task IWorkQueue.EnqueueAsyncWork(IAsyncWork work, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, 0);
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, 0);
        return task;
    }

#endregion

#region Priority

    Task IPriorityWorkQueue.EnqueueWork(IWork work, int priority, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, priority);
        return task;
    }

    Task<TResult> IPriorityWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, int priority, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, priority);
        return task;
    }

    Task IPriorityWorkQueue.EnqueueAsyncWork(IAsyncWork work, int priority, int attemptsCount, CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, priority);
        return task;
    }

    Task<TResult> IPriorityWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, int priority, int attemptsCount,
        CancellationToken cancellation)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper, priority);
        return task;
    }

#endregion
}
