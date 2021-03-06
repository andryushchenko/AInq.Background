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

using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Wrappers.WorkWrapperFactory;

namespace AInq.Background.Managers
{

/// <summary> Background work queue manager </summary>
public sealed class WorkQueueManager : TaskManager<object?>, IWorkQueue
{
    private readonly int _maxAttempts;

    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public WorkQueueManager(int maxAttempts = int.MaxValue)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IWorkQueue.MaxAttempts => _maxAttempts;

    Task IWorkQueue.EnqueueWork(IWork work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper);
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper);
        return task;
    }

    Task IWorkQueue.EnqueueAsyncWork(IAsyncWork work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper);
        return task;
    }

    Task<TResult> IWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation, int attemptsCount)
    {
        var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), FixAttempts(attemptsCount), cancellation);
        AddTask(workWrapper);
        return task;
    }

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}

}
