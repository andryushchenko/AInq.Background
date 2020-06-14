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

using AInq.Background.Wrappers;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Managers
{

internal class ConveyorManager<TData, TResult> : IConveyor<TData, TResult>, ITaskManager<IConveyorMachine<TData, TResult>, object?>
{
    protected readonly AsyncAutoResetEvent NewDataEvent = new AsyncAutoResetEvent(false);
    protected readonly ConcurrentQueue<ITaskWrapper<IConveyorMachine<TData, TResult>>> Queue = new ConcurrentQueue<ITaskWrapper<IConveyorMachine<TData, TResult>>>();
    private readonly int _maxAttempts;

    internal ConveyorManager(int maxAttempts = int.MaxValue)
    {
        _maxAttempts = Math.Max(maxAttempts, 1);
    }

    int IConveyor<TData, TResult>.MaxAttempts => _maxAttempts;

    Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
    {
        var element = new ConveyorDataWrapper<TData, TResult>(data, cancellation, FixAttempts(attemptsCount));
        Queue.Enqueue(element);
        NewDataEvent.Set();
        return element.Result;
    }

    bool ITaskManager<IConveyorMachine<TData, TResult>, object?>.HasTask => !Queue.IsEmpty;

    Task ITaskManager<IConveyorMachine<TData, TResult>, object?>.WaitForTaskAsync(CancellationToken cancellation)
        => Queue.IsEmpty
            ? NewDataEvent.WaitAsync(cancellation)
            : Task.CompletedTask;

    public (ITaskWrapper<IConveyorMachine<TData, TResult>>?, object?) GetTask()
        => (Queue.TryDequeue(out var task)
                ? task
                : null, null);

    void ITaskManager<IConveyorMachine<TData, TResult>, object?>.RevertTask(ITaskWrapper<IConveyorMachine<TData, TResult>> task, object? metadata)
        => Queue.Enqueue(task);

    protected int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}

}
