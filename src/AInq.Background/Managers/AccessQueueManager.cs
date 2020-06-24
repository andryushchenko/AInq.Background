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
using AInq.Background.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Tasks.AccessFactory;
using static AInq.Background.Wrappers.AccessWrapperFactory;

namespace AInq.Background.Managers
{

internal sealed class AccessQueueManager<TResource> : TaskManager<TResource>, IAccessQueue<TResource>
{
    private readonly int _maxAttempts;

    public AccessQueueManager(int maxAttempts = int.MaxValue)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IAccessQueue<TResource>.MaxAttempts => _maxAttempts;

    Task IAccessQueue<TResource>.EnqueueAccess(IAccess<TResource> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAccess<TAccess>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(CreateAccess<TResource>((resource, provider) => provider.GetRequiredService<TAccess>().Access(resource, provider)),
                FixAttempts(attemptsCount),
                cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAccess<TResult>(IAccess<TResource, TResult> access, CancellationToken cancellation,
        int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAccess<TAccess, TResult>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(
                CreateAccess<TResource, TResult>((resource, provider) => provider.GetRequiredService<TAccess>().Access(resource, provider)),
                FixAttempts(attemptsCount),
                cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAsyncAccess(IAsyncAccess<TResource> access, CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task IAccessQueue<TResource>.EnqueueAsyncAccess<TAsyncAccess>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(
                CreateAsyncAccess<TResource>((resource, provider, token)
                    => provider.GetRequiredService<TAsyncAccess>().AccessAsync(resource, provider, token)),
                FixAttempts(attemptsCount),
                cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, CancellationToken cancellation,
        int attemptsCount)
    {
        var (accessWrapper, task) =
            CreateAccessWrapper(access ?? throw new ArgumentNullException(nameof(access)), FixAttempts(attemptsCount), cancellation);
        AddTask(accessWrapper);
        return task;
    }

    Task<TResult> IAccessQueue<TResource>.EnqueueAsyncAccess<TAsyncAccess, TResult>(CancellationToken cancellation, int attemptsCount)
    {
        var (accessWrapper, task) = CreateAccessWrapper(
            CreateAsyncAccess<TResource, TResult>((resource, provider, token)
                => provider.GetRequiredService<TAsyncAccess>().AccessAsync(resource, provider, token)),
            FixAttempts(attemptsCount),
            cancellation);
        AddTask(accessWrapper);
        return task;
    }

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}

}
