// Copyright 2021 Anton Andryushchenko
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Processors
{

internal sealed class MultipleNullProcessor<TMetadata> : ITaskProcessor<object?, TMetadata>
{
    private readonly SemaphoreSlim _semaphore;

    internal MultipleNullProcessor(int maxParallelTasks)
        => _semaphore = new SemaphoreSlim(Math.Max(1, maxParallelTasks));

    async Task ITaskProcessor<object?, TMetadata>.ProcessPendingTasksAsync(ITaskManager<object?, TMetadata> manager, IServiceProvider provider,
        ILogger? logger, CancellationToken cancellation)
    {
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            var (task, metadata) = manager.GetTask();
            if (task == null)
                continue;
            await _semaphore.WaitAsync(cancellation).ConfigureAwait(false);
            Task.Run(async () =>
                    {
                        using var taskScope = provider.CreateScope();
                        if (!await task.ExecuteAsync(null, taskScope.ServiceProvider, logger, cancellation).ConfigureAwait(false))
                            manager.RevertTask(task, metadata);
                        _semaphore.Release();
                    },
                    cancellation)
                .Ignore();
        }
    }
}

}
