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
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Managers
{
    internal class DataConveyorManager<TData, TResult> : IDataConveyor<TData, TResult>, ITaskQueueManager<IDataConveyorMachine<TData, TResult>, object>
    {
        protected readonly AsyncAutoResetEvent NewDataEvent = new AsyncAutoResetEvent(false);
        protected readonly ConcurrentQueue<ITaskWrapper<IDataConveyorMachine<TData, TResult>>> Queue = new ConcurrentQueue<ITaskWrapper<IDataConveyorMachine<TData, TResult>>>();

        Task<TResult> IDataConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var element = new DataConveyorElement<TData, TResult>(data, cancellation, attemptsCount);
            Queue.Enqueue(element);
            NewDataEvent.Set();
            return element.Result;
        }

        bool ITaskQueueManager<IDataConveyorMachine<TData, TResult>, object>.HasTask => !Queue.IsEmpty;

        Task ITaskQueueManager<IDataConveyorMachine<TData, TResult>, object>.WaitForTaskAsync(CancellationToken cancellation)
            => Queue.IsEmpty
                ? NewDataEvent.WaitAsync(cancellation)
                : Task.CompletedTask;

        public (ITaskWrapper<IDataConveyorMachine<TData, TResult>>, object) GetTask()
            => (Queue.TryDequeue(out var task)
                ? task
                : null, null);

        void ITaskQueueManager<IDataConveyorMachine<TData, TResult>, object>.RevertTask(ITaskWrapper<IDataConveyorMachine<TData, TResult>> task, object metadata)
            => Queue.Enqueue(task);
    }
}