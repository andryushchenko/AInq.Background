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
using AInq.Support.Background.WorkElements;
using Microsoft.Extensions.DependencyInjection;
using static AInq.Support.Background.WorkElements.WorkWrapperFactory;
using static AInq.Support.Background.WorkElements.WorkFactory;

namespace AInq.Support.Background.WorkQueue
{
    internal sealed class PriorityWorkQueueManager : WorkQueueManager, IPriorityWorkQueue
    {
        private readonly int _maxPriority;
        internal IReadOnlyList<ConcurrentQueue<IWorkWrapper>> Queues { get; }

        int IPriorityWorkQueue.MaxPriority => _maxPriority;

        internal PriorityWorkQueueManager(int maxPriority)
        {
            if (maxPriority < 0) throw new ArgumentOutOfRangeException(nameof(maxPriority));
            _maxPriority = maxPriority;
            var queues = new ConcurrentQueue<IWorkWrapper>[_maxPriority + 1];
            queues[0] = Queue;
            for (var index = 1; index <= _maxPriority; index++)
                queues[index] = new ConcurrentQueue<IWorkWrapper>();
            Queues = queues;
        }

        Task IPriorityWorkQueue.EnqueueWork(IWork work, int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IPriorityWorkQueue.EnqueueWork<TWork>(int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueWork<TWork, TResult>(int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IPriorityWorkQueue.EnqueueAsyncWork(IAsyncWork work, int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IPriorityWorkQueue.EnqueueAsyncWork<TWork>(int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueAsyncWork<TWork, TResult>(int priority, CancellationToken cancellation)
        {
            if (priority < 0 || priority > _maxPriority) throw new ArgumentOutOfRangeException(nameof(priority));
            var (workWrapper, task) = CreateWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), cancellation);
            Queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }
    }
}