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

    internal MultipleReusableProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxArgumentsCount)
    {
        _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        _maxArgumentCount = Math.Max(1, maxArgumentsCount);
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
                        logger?.LogError(ex, "Error creating argument {Type} with {Fabric}", typeof(TArgument), _argumentFabric);
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
                    while (!_reusable.IsEmpty && _reusable.TryTake(out var result))
                    {
                        var argument = result;
                        Interlocked.Decrement(ref _currentArgumentCount);
                        _reset.Set();
                        _ = Task.Run(async () =>
                            {
                                var activatable = argument as IActivatable;
                                try
                                {
                                    if (activatable != null && activatable.IsActive)
                                        await activatable.DeactivateAsync(cancellation);
                                }
                                catch (Exception ex)
                                {
                                    logger?.LogError(ex, "Error stopping stoppable argument {Argument}", activatable);
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
