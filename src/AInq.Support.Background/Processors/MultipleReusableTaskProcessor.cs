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
    internal sealed class MultipleReusableTaskProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
    {
        private readonly Func<IServiceProvider, TArgument> _argumentFabric;
        private readonly AsyncAutoResetEvent _reset = new AsyncAutoResetEvent(false);
        private readonly ConcurrentBag<TArgument> _reusable = new ConcurrentBag<TArgument>();
        private readonly int _maxArgumentCount;
        private int _currentArgumentCount;


        internal MultipleReusableTaskProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxSimultaneousTasks)
        {
            if (maxSimultaneousTasks < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneousTasks), maxSimultaneousTasks, null);
            _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
            _maxArgumentCount = maxSimultaneousTasks;
        }

        async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
        {
            var currentTasks = new LinkedList<Task>();
            while (manager.HasTask)
            {
                var taskScope = provider.CreateScope();
                if (!_reusable.TryTake(out var argument))
                {
                    if (_currentArgumentCount < _maxArgumentCount)
                    {
                        argument = _argumentFabric.Invoke(taskScope.ServiceProvider);
                        Interlocked.Increment(ref _currentArgumentCount);
                    }
                    else
                    {
                        await _reset.WaitAsync(cancellation);
                        continue;
                    }
                }
                var (task, metadata) = manager.GetTask();
                if (task == null)
                    return;
                currentTasks.AddLast(Task.Run(async () =>
                {
                    try
                    {
                        if (argument is IStoppableTaskMachine machine && !machine.IsRunning)
                            await machine.StartMachineAsync(cancellation);
                        if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, cancellation))
                            manager.RevertTask(task, metadata);
                        if (manager.HasTask && argument is IThrottlingTaskMachine throttling && throttling.Timeout.Ticks > 0)
                            await Task.Delay(throttling.Timeout, cancellation);
                    }
                    finally
                    {
                        _reusable.Add(argument);
                        _reset.Set();
                        taskScope.Dispose();
                    }
                }, cancellation));
            }
            _ = Task.WhenAll(currentTasks).ContinueWith(task =>
            {
                while (_reusable.TryTake(out var argument))
                {
                    Interlocked.Decrement(ref _currentArgumentCount);
                    if (argument is IStoppableTaskMachine machine && machine.IsRunning)
                        _ = machine.StopMachineAsync(cancellation);
                }
            }, cancellation);
        }
    }
}