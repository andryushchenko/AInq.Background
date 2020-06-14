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

using AInq.Background.Wrappers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Managers
{

internal sealed class PriorityConveyorManager<TData, TResult> : ConveyorManager<TData, TResult>, IPriorityConveyor<TData, TResult>, ITaskManager<IConveyorMachine<TData, TResult>, int>
{
    private readonly int _maxPriority;

    private readonly IList<ConcurrentQueue<ITaskWrapper<IConveyorMachine<TData, TResult>>>> _queues;

    internal PriorityConveyorManager(int maxPriority = 100, int maxAttempts = int.MaxValue) : base(maxAttempts)
    {
        _maxPriority = Math.Min(100, Math.Max(1, maxPriority));
        _queues = new ConcurrentQueue<ITaskWrapper<IConveyorMachine<TData, TResult>>>[_maxPriority + 1];
        _queues[0] = Queue;
        for (var index = 1; index <= _maxPriority; index++)
            _queues[index] = new ConcurrentQueue<ITaskWrapper<IConveyorMachine<TData, TResult>>>();
    }

    int IPriorityConveyor<TData, TResult>.MaxPriority => _maxPriority;

    bool ITaskManager<IConveyorMachine<TData, TResult>, int>.HasTask => _queues.Any(queue => !queue.IsEmpty);

    Task<TResult> IPriorityConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, CancellationToken cancellation, int attemptsCount)
    {
        var element = new ConveyorDataWrapper<TData, TResult>(data, cancellation, FixAttempts(attemptsCount));
        _queues[FixPriority(priority)].Enqueue(element);
        NewDataEvent.Set();
        return element.Result;
    }

    Task ITaskManager<IConveyorMachine<TData, TResult>, int>.WaitForTaskAsync(CancellationToken cancellation)
        => _queues.Any(queue => !queue.IsEmpty)
            ? Task.CompletedTask
            : NewDataEvent.WaitAsync(cancellation);

    (ITaskWrapper<IConveyorMachine<TData, TResult>>?, int) ITaskManager<IConveyorMachine<TData, TResult>, int>.GetTask()
    {
        while (true)
        {
            var pendingQueue = _queues.FirstOrDefault(queue => !queue.IsEmpty);
            if (pendingQueue == null || !pendingQueue.TryDequeue(out var task))
                return (null, -1);
            if (!task.IsCanceled)
                return (task, _queues.IndexOf(pendingQueue));
        }
    }

    void ITaskManager<IConveyorMachine<TData, TResult>, int>.RevertTask(ITaskWrapper<IConveyorMachine<TData, TResult>> task, int metadata)
    {
        _queues[FixPriority(metadata)].Enqueue(task);
        NewDataEvent.Set();
    }

    private int FixPriority(int priority)
        => Math.Min(_maxPriority, Math.Max(0, priority));
}

}
