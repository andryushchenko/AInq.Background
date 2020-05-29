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

using AInq.Support.Background.Elements;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Managers
{
    internal sealed class PriorityDataConveyorManager<TData, TResult> : DataConveyorManager<TData, TResult>, IPriorityDataConveyor<TData, TResult>, ITaskQueueManager<IDataConveyorMachine<TData, TResult>, int>
    {
        private readonly int _maxPriority;

        private readonly IList<ConcurrentQueue<ITaskWrapper<IDataConveyorMachine<TData, TResult>>>> _queues;

        internal PriorityDataConveyorManager(int maxPriority)
        {
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            _maxPriority = maxPriority;
            var queues = new ConcurrentQueue<ITaskWrapper<IDataConveyorMachine<TData, TResult>>>[_maxPriority + 1];
            queues[0] = Queue;
            for (var index = 1; index <= _maxPriority; index++) queues[index] = new ConcurrentQueue<ITaskWrapper<IDataConveyorMachine<TData, TResult>>>();
            _queues = queues;
        }

        int IPriorityDataConveyor<TData, TResult>.MaxPriority => _maxPriority;

        bool ITaskQueueManager<IDataConveyorMachine<TData, TResult>, int>.HasTask => _queues.Any(queue => !queue.IsEmpty);

        Task<TResult> IPriorityDataConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var element = new DataConveyorElement<TData, TResult>(data, cancellation, attemptsCount);
            _queues[priority].Enqueue(element);
            NewDataEvent.Set();
            return element.Result;
        }


        Task ITaskQueueManager<IDataConveyorMachine<TData, TResult>, int>.WaitForTaskAsync(CancellationToken cancellation)
            => _queues.Any(queue => !queue.IsEmpty)
                ? Task.CompletedTask
                : NewDataEvent.WaitAsync(cancellation);

        (ITaskWrapper<IDataConveyorMachine<TData, TResult>>, int) ITaskQueueManager<IDataConveyorMachine<TData, TResult>, int>.GetTask()
        {
            var pendingQueue = _queues.FirstOrDefault(queue => !queue.IsEmpty);
            return pendingQueue != null && pendingQueue.TryDequeue(out var task)
                ? (task, _queues.IndexOf(pendingQueue))
                : (null, -1);
        }

        void ITaskQueueManager<IDataConveyorMachine<TData, TResult>, int>.RevertTask(ITaskWrapper<IDataConveyorMachine<TData, TResult>> task, int metadata)
        {
            if (metadata < 0 || metadata > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(metadata), metadata, null);
            _queues[metadata].Enqueue(task);
            NewDataEvent.Set();
        }
    }
}