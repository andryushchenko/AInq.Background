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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Processors
{
    internal sealed class MultipleStaticTaskProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
    {
        private readonly ConcurrentBag<TArgument> _inactive;
        private readonly ConcurrentBag<TArgument> _active;
        private readonly SemaphoreSlim _semaphore;
        private readonly Func<ITaskQueueManager<TArgument, TMetadata>, IServiceProvider, CancellationToken, Task> _processor;

        internal MultipleStaticTaskProcessor(IEnumerable<TArgument> arguments)
        {
            _inactive = new ConcurrentBag<TArgument>(arguments);
            if (_inactive.IsEmpty)
                throw new ArgumentException("Empty collection", nameof(arguments));
            if (typeof(IStoppableTaskMachine).IsAssignableFrom(typeof(TArgument)))
            {
                _processor = ProcessWithStartStopAsync;
                _active = new ConcurrentBag<TArgument>();
            }
            else
            {
                _processor = ProcessAsync;
                _active = _inactive;
            }
            _semaphore = new SemaphoreSlim(_inactive.Count);
        }

        async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskQueueManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
            => await _processor.Invoke(manager, provider, cancellation);

        private async Task ProcessWithStartStopAsync(ITaskQueueManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
        {
            while (manager.HasTask)
            {
                await _semaphore.WaitAsync(cancellation);
                var (task, metadata) = manager.GetTask();
                if (task == null)
                {
                    _semaphore.Release();
                    break;
                }
                if (_active.TryTake(out var argument) || _inactive.TryTake(out argument))
                {
                    _ = Task.Run(async () =>
                    {
                        if (argument is IStoppableTaskMachine startingMachine && !startingMachine.IsRunning)
                            await startingMachine.StartMachineAsync(cancellation);
                        if (!await task.ExecuteAsync(argument, provider, cancellation))
                            manager.RevertTask(task, metadata);
                        if (manager.HasTask)
                        {
                            if (manager.HasTask && argument is IThrottlingTaskMachine throttling && throttling.Timeout.Ticks > 0)
                                await Task.Delay(throttling.Timeout, cancellation);
                            _active.Add(argument);
                        }
                        else
                        {
                            if (argument is IStoppableTaskMachine stoppingMachine && stoppingMachine.IsRunning)
                                await stoppingMachine.StopMachineAsync(cancellation);
                            _inactive.Add(argument);
                        }
                        _semaphore.Release();
                    }, cancellation);
                }
                else
                {
                    manager.RevertTask(task, metadata);
                    _semaphore.Release();
                }
            }
        }

        private async Task ProcessAsync(ITaskQueueManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
        {
            while (manager.HasTask)
            {
                await _semaphore.WaitAsync(cancellation);
                var (task, metadata) = manager.GetTask();
                if (task == null)
                {
                    _semaphore.Release();
                    break;
                }
                if (_inactive.TryTake(out var argument))
                {
                    _ = Task.Run(async () =>
                    {
                        if (!await task.ExecuteAsync(argument, provider, cancellation))
                            manager.RevertTask(task, metadata);
                        _inactive.Add(argument);
                        if (manager.HasTask && argument is IThrottlingTaskMachine throttling && throttling.Timeout.Ticks > 0)
                            await Task.Delay(throttling.Timeout, cancellation);
                        _semaphore.Release();
                    }, cancellation);
                }
                else
                {
                    manager.RevertTask(task, metadata);
                    _semaphore.Release();
                }
            }
        }
    }
}