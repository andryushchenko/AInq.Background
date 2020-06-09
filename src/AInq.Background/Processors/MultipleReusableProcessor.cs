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

using AInq.Background.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Processors
{

internal sealed class MultipleReusableProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
{
    private readonly Func<IServiceProvider, TArgument> _argumentFabric;
    private readonly AsyncAutoResetEvent _reset = new AsyncAutoResetEvent(false);
    private readonly ConcurrentBag<TArgument> _reusable = new ConcurrentBag<TArgument>();
    private readonly int _maxArgumentCount;
    private int _currentArgumentCount;

    internal MultipleReusableProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxSimultaneousTasks)
    {
        _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        _maxArgumentCount = maxSimultaneousTasks < 1
            ? throw new ArgumentOutOfRangeException(nameof(maxSimultaneousTasks), maxSimultaneousTasks, null)
            : maxSimultaneousTasks;
    }

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider, ILogger? logger, CancellationToken cancellation)
    {
        var currentTasks = new LinkedList<Task>();
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            var taskScope = provider.CreateScope();
            if (!_reusable.TryTake(out var argument))
            {
                if (_currentArgumentCount < _maxArgumentCount)
                {
                    try
                    {
                        argument = _argumentFabric.Invoke(taskScope.ServiceProvider);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex, "Error creating argument {1} with {0}", _argumentFabric, typeof(TArgument));
                        continue;
                    }
                    Interlocked.Increment(ref _currentArgumentCount);
                }
                else
                {
                    if (_reusable.IsEmpty)
                        await _reset.WaitAsync(cancellation);
                    continue;
                }
            }
            var (task, metadata) = manager.GetTask();
            if (task == null)
            {
                _reusable.Add(argument);
                continue;
            }
            currentTasks.AddLast(Task.Run(async () =>
                {
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
                        Interlocked.Decrement(ref _currentArgumentCount);
                        _reset.Set();
                        return;
                    }
                    if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, logger, cancellation))
                        manager.RevertTask(task, metadata);
                    try
                    {
                        if (manager.HasTask && argument is IThrottling throttling && throttling.Timeout.Ticks > 0)
                            await Task.Delay(throttling.Timeout, cancellation);
                    }
                    finally
                    {
                        _reusable.Add(argument);
                        _reset.Set();
                        taskScope.Dispose();
                    }
                },
                cancellation));
        }
        Task.WhenAll(currentTasks)
            .ContinueWith(task =>
                {
                    while (!_reusable.IsEmpty && _reusable.TryTake(out var argument))
                    {
                        var active = argument;
                        Interlocked.Decrement(ref _currentArgumentCount);
                        _reset.Set();
                        _ = Task.Run(async () =>
                            {
                                var machine = active as IStoppable;
                                try
                                {
                                    if (machine != null && machine.IsRunning)
                                        await machine.StopMachineAsync(cancellation);
                                }
                                catch (Exception ex)
                                {
                                    logger?.LogError(ex, "Error stopping machine {0}", machine);
                                }
                            },
                            cancellation);
                    }
                },
                cancellation)
            .Ignore();
    }
}

}
