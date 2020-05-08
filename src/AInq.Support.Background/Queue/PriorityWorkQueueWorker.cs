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

namespace AInq.Support.Background.Queue
{
    internal sealed class PriorityWorkQueueWorker : WorkQueueWorker
    {
        private readonly PriorityWorkQueueManager _priorityWorkQueueManager;

        internal PriorityWorkQueueWorker(PriorityWorkQueueManager priorityWorkQueueManager, IServiceProvider provider)
            : base(priorityWorkQueueManager, provider)
        {
            _priorityWorkQueueManager = priorityWorkQueueManager;
        }

        protected override async Task<bool> GetNextWorkAsync()
        {
            var currentQueue = _priorityWorkQueueManager.Queues.Reverse().FirstOrDefault(queue => !queue.IsEmpty);
            if (currentQueue == null) return false;
            if (!currentQueue.TryDequeue(out var work)) return false;
            await DoWorkAsync(work);
            return !_priorityWorkQueueManager.Queues.All(queue => queue.IsEmpty);
        }
    }
}