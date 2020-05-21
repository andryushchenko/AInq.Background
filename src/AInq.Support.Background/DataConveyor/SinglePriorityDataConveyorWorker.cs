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
using System.Linq;
using System.Threading.Tasks;

namespace AInq.Support.Background.DataConveyor
{
    internal sealed class SinglePriorityDataConveyorWorker<TData, TResult> : SingleDataConveyorWorker<TData, TResult>
    {
        private readonly PriorityDataConveyorManager<TData, TResult> _conveyorManager;

        internal SinglePriorityDataConveyorWorker(PriorityDataConveyorManager<TData, TResult> conveyorManager, IDataConveyorMachine<TData, TResult> machine) : base(conveyorManager, machine)
        {
            _conveyorManager = conveyorManager ?? throw new ArgumentNullException(nameof(conveyorManager));
        }

        protected override async Task<bool> ProcessNextElementAsync()
        {
            var currentQueue = _conveyorManager.Queues.Reverse().FirstOrDefault(queue => !queue.IsEmpty);
            if (currentQueue == null) return false;
            if (!currentQueue.TryDequeue(out var element)) return false;
            if (await ProcessElementAsync(element)) return !_conveyorManager.Queues.All(queue => queue.IsEmpty);
            currentQueue.Enqueue(element);
            return true;
        }
    }
}