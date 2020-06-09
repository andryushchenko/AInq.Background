﻿/*
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

    internal MultipleOneTimeProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxSimultaneousTasks)
    {
        _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        _semaphore = new SemaphoreSlim(maxSimultaneousTasks < 1
            ? throw new ArgumentOutOfRangeException(nameof(maxSimultaneousTasks), maxSimultaneousTasks, null)
            : maxSimultaneousTasks);
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
                            logger?.LogError(ex, "Error creating argument {1} with {0}", _argumentFabric, typeof(TArgument));
                            manager.RevertTask(task, metadata);
                            _semaphore.Release();
                            return;
                        }
                        var machine = argument as IStoppable;
                        try
                        {
                            if (machine != null && !machine.IsRunning)
                                await machine.StartMachineAsync(cancellation);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error starting machine {0}", machine);
                            manager.RevertTask(task, metadata);
                            _semaphore.Release();
                            return;
                        }
                        if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, logger, cancellation))
                            manager.RevertTask(task, metadata);
                        try
                        {
                            if (machine != null && machine.IsRunning)
                                await machine.StopMachineAsync(cancellation);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error stopping machine {0}", machine);
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