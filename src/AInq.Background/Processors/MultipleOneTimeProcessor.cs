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

using AInq.Background.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace AInq.Background.Processors
{

internal class MultipleOneTimeProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
{
    private readonly Func<IServiceProvider, TArgument> _argumentFabric;
    private readonly SemaphoreSlim _semaphore;

    internal MultipleOneTimeProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxParallelTasks)
    {
        _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        _semaphore = new SemaphoreSlim(Math.Max(1, maxParallelTasks));
    }

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider, ILogger? logger, CancellationToken cancellation)
    {
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            var (task, metadata) = manager.GetTask();
            if (task == null)
                continue;
            await _semaphore.WaitAsync(cancellation);
            Task.Run(async () =>
                    {
                        using var taskScope = provider.CreateScope();
                        TArgument argument;
                        try
                        {
                            argument = _argumentFabric.Invoke(taskScope.ServiceProvider);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error creating argument {Type} with {Fabric}", typeof(TArgument), _argumentFabric);
                            manager.RevertTask(task, metadata);
                            _semaphore.Release();
                            return;
                        }
                        var activatable = argument as IActivatable;
                        try
                        {
                            if (activatable != null && !activatable.IsActive)
                                await activatable.ActivateAsync(cancellation);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error starting stoppable argument {Argument}", activatable);
                            manager.RevertTask(task, metadata);
                            _semaphore.Release();
                            return;
                        }
                        if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, logger, cancellation))
                            manager.RevertTask(task, metadata);
                        try
                        {
                            if (activatable != null && activatable.IsActive)
                                await activatable.DeactivateAsync(cancellation);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error stopping stoppable argument {Argument}", activatable);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    },
                    cancellation)
                .Ignore();
        }
    }
}

}
