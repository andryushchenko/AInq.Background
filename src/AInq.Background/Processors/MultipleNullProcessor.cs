﻿// Copyright 2020 Anton Andryushchenko
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

using AInq.Background.Managers;
using AInq.Background.Wrappers;

namespace AInq.Background.Processors;

internal sealed class MultipleNullProcessor<TMetadata> : ITaskProcessor<object?, TMetadata>
{
    private readonly SemaphoreSlim _semaphore;

    internal MultipleNullProcessor(int maxParallelTasks)
        => _semaphore = new SemaphoreSlim(Math.Max(1, maxParallelTasks));

    async Task ITaskProcessor<object?, TMetadata>.ProcessPendingTasksAsync(ITaskManager<object?, TMetadata> manager, IServiceProvider provider,
        ILogger logger, CancellationToken cancellation)
    {
        _ = manager ?? throw new ArgumentNullException(nameof(manager));
        _ = provider ?? throw new ArgumentNullException(nameof(provider));
        _ = logger ?? throw new ArgumentNullException(nameof(logger));
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            var (task, metadata) = manager.GetTask();
            if (task == null) continue;
            await _semaphore.WaitAsync(cancellation).ConfigureAwait(false);
            Execute(task, metadata, manager, provider, logger, cancellation);
        }
    }

    private async void Execute(ITaskWrapper<object?> task, TMetadata metadata, ITaskManager<object?, TMetadata> manager, IServiceProvider provider,
        ILogger logger, CancellationToken cancellation)
    {
        if (!await task.ExecuteAsync(null, provider, logger, cancellation).ConfigureAwait(false))
            manager.RevertTask(task, metadata);
        _semaphore.Release();
    }
}
