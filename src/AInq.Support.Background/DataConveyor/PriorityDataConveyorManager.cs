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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.DataConveyor
{
    internal sealed class PriorityDataConveyorManager<TData, TResult> : DataConveyorManager<TData, TResult>, IPriorityDataConveyor<TData, TResult>
    {
        private readonly int _maxPriority;
        internal IReadOnlyList<ConcurrentQueue<DataConveyorElement<TData, TResult>>> Queues { get; }

        internal PriorityDataConveyorManager(int maxPriority)
        {
            if (maxPriority < 0) throw new ArgumentOutOfRangeException(nameof(maxPriority));
            _maxPriority = maxPriority;
            var queues = new ConcurrentQueue<DataConveyorElement<TData, TResult>>[_maxPriority + 1];
            queues[0] = Queue;
            for (var index = 1; index <= _maxPriority; index++) queues[index] = new ConcurrentQueue<DataConveyorElement<TData, TResult>>();
            Queues = queues;
        }

        int IPriorityDataConveyor<TData, TResult>.MaxPriority => _maxPriority;

        Task<TResult> IPriorityDataConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            if (attemptsCount <= 0) throw new ArgumentOutOfRangeException(nameof(attemptsCount));
            var element = new DataConveyorElement<TData, TResult>(data, cancellation, attemptsCount);
            Queues[priority].Enqueue(element);
            NewDataEvent.Set();
            return element.Result;
        }
    }
}