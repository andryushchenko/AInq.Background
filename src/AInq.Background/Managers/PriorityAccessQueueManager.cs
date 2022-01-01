// Copyright 2020-2022 Anton Andryushchenko
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

using static AInq.Background.Wrappers.AccessWrapperFactory;

namespace AInq.Background.Managers;

/// <summary> Background access queue manager with numeric prioritization </summary>
/// <typeparam name="TResource"> Shared resource type </typeparam>
public sealed class PriorityAccessQueueManager<TResource> : PriorityTaskManager<TResource>, IPriorityAccessQueue<TResource>
    where TResource : notnull
{
    private readonly int _maxAttempts;

    /// <param name="maxPriority"> Max allowed work priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public PriorityAccessQueueManager(int maxPriority = 100, int maxAttempts = int.MaxValue) : base(maxPriority)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IPriorityAccessQueue<TResource>.MaxPriority => MaxPriority;
    int IAccessQueue<TResource>.MaxAttempts => _maxAttempts;

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));

#region Base

    Task IAccessQueue<TResource>.EnqueueAccess(IAccess<TResource> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, 0);
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAccess<TResult>(IAccess<TResource, TResult> access, CancellationToken cancellation,
        int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, 0);
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAsyncAccess(IAsyncAccess<TResource> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, 0);
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, CancellationToken cancellation,
        int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, 0);
        return task;
    }

#endregion

#region Priority

    Task IPriorityAccessQueue<TResource>.EnqueueAccess(IAccess<TResource> access, int priority, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, priority);
        return task;
    }

    Task<TResult> IPriorityAccessQueue<TResource>.EnqueueAccess<TResult>(IAccess<TResource, TResult> access, int priority,
        CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, priority);
        return task;
    }

    Task IPriorityAccessQueue<TResource>.EnqueueAsyncAccess(IAsyncAccess<TResource> access, int priority, CancellationToken cancellation,
        int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, priority);
        return task;
    }

    Task<TResult> IPriorityAccessQueue<TResource>.EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, int priority,
        CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper, priority);
        return task;
    }

#endregion
}
