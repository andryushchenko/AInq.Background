// Copyright 2020-2021 Anton Andryushchenko
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
using AInq.Background.Tasks;
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

internal sealed class MultipleStaticProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
{
    private readonly ConcurrentBag<TArgument> _active;
    private readonly ConcurrentBag<TArgument> _inactive;
    private readonly AsyncAutoResetEvent _reset = new(false);

    internal MultipleStaticProcessor(IEnumerable<TArgument> arguments)
    {
        _inactive = new ConcurrentBag<TArgument>(arguments);
        if (_inactive.IsEmpty)
            throw new ArgumentException("Empty collection", nameof(arguments));
        _active = typeof(IStartStoppable).IsAssignableFrom(typeof(TArgument)) ? new ConcurrentBag<TArgument>() : _inactive;
    }

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider,
        ILogger logger, CancellationToken cancellation)
    {
        var currentTasks = new LinkedList<Task>();
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            if (!_active.TryTake(out var argument) && !_inactive.TryTake(out argument))
            {
                if (_active.IsEmpty && _inactive.IsEmpty)
                    await _reset.WaitAsync(cancellation).ConfigureAwait(false);
                continue;
            }
            var startStoppable = argument as IStartStoppable;
            var (task, metadata) = manager.GetTask();
            if (task == null)
            {
                if (startStoppable is { IsActive: true })
                    _active.Add(argument);
                else _inactive.Add(argument);
                _reset.Set();
                continue;
            }
            currentTasks.AddLast(Task.Run(async () =>
                {
                    try
                    {
                        if (startStoppable is { IsActive: false })
                            await startStoppable.ActivateAsync(cancellation).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error starting stoppable argument {Argument}", startStoppable);
                        manager.RevertTask(task, metadata);
                        _inactive.Add(argument);
                        _reset.Set();
                        return;
                    }
                    using var taskScope = provider.CreateScope();
                    if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, logger, cancellation).ConfigureAwait(false))
                        manager.RevertTask(task, metadata);
                    try
                    {
                        if (manager.HasTask && argument is IThrottling { Timeout: { Ticks: > 0 } } throttling)
                            await Task.Delay(throttling.Timeout, cancellation).ConfigureAwait(false);
                    }
                    finally
                    {
                        if (startStoppable is { IsActive: true })
                            _active.Add(argument);
                        else _inactive.Add(argument);
                        _reset.Set();
                    }
                },
                cancellation));
        }
        Task.WhenAll(currentTasks)
            .ContinueWith(_ =>
                {
                    while (!_active.IsEmpty && _active.TryTake(out var result))
                    {
                        var argument = result;
                        Task.Run(async () =>
                                {
                                    var startStoppable = argument as IStartStoppable;
                                    try
                                    {
                                        if (startStoppable is { IsActive: true })
                                            await startStoppable.DeactivateAsync(cancellation).ConfigureAwait(false);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, "Error stopping stoppable argument {Argument}", startStoppable);
                                    }
                                    finally
                                    {
                                        _inactive.Add(argument);
                                        _reset.Set();
                                    }
                                },
                                cancellation)
                            .Ignore();
                    }
                },
                cancellation)
            .Ignore();
    }
}

}
