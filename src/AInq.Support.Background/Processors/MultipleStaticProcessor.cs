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

using AInq.Support.Background.Managers;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Processors
{
    internal sealed class MultipleStaticProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
    {
        private readonly ConcurrentBag<TArgument> _inactive;
        private readonly ConcurrentBag<TArgument> _active;
        private readonly AsyncAutoResetEvent _reset = new AsyncAutoResetEvent(false);

        internal MultipleStaticProcessor(IEnumerable<TArgument> arguments)
        {
            _inactive = new ConcurrentBag<TArgument>(arguments);
            if (_inactive.IsEmpty)
                throw new ArgumentException("Empty collection", nameof(arguments));
            _active = typeof(IStoppable).IsAssignableFrom(typeof(TArgument))
                ? new ConcurrentBag<TArgument>()
                : _inactive;
        }

        async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
        {
            var currentTasks = new LinkedList<Task>();
            while (manager.HasTask)
            {
                if (!_active.TryTake(out var argument) && !_inactive.TryTake(out argument))
                {
                    await _reset.WaitAsync(cancellation);
                    continue;
                }
                var stoppable = argument as IStoppable;
                var (task, metadata) = manager.GetTask();
                if (task == null)
                {
                    if (stoppable != null && stoppable.IsRunning)
                        _active.Add(argument);
                    else _inactive.Add(argument);
                    _reset.Set();
                    return;
                }
                currentTasks.AddLast(Task.Run(async () =>
                {
                    try
                    {
                        if (stoppable != null && !stoppable.IsRunning)
                            await stoppable.StartMachineAsync(cancellation);
                        using var taskScope = provider.CreateScope();
                        if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, cancellation))
                            manager.RevertTask(task, metadata);
                        if (manager.HasTask)
                        {
                            if (argument is IThrottling throttling && throttling.Timeout.Ticks > 0)
                                await Task.Delay(throttling.Timeout, cancellation);
                        }
                        else
                        {
                            if (stoppable != null && stoppable.IsRunning)
                                await stoppable.StopMachineAsync(cancellation);
                        }
                    }
                    finally
                    {
                        if (stoppable != null && stoppable.IsRunning)
                            _active.Add(argument);
                        else _inactive.Add(argument);
                        _reset.Set();
                    }
                }, cancellation));
            }
            _ = Task.WhenAll(currentTasks).ContinueWith(task =>
            {
                while (_active.TryTake(out var argument))
                {
                    var active = argument;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (active is IStoppable stoppable && stoppable.IsRunning)
                                await stoppable.StopMachineAsync(cancellation);
                        }
                        finally
                        {
                            _inactive.Add(active);
                            _reset.Set();
                        }
                    }, cancellation);
                }
            }, cancellation);
        }
    }
}