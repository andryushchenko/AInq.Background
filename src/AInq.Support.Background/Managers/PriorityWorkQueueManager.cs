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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Support.Background.Elements.WorkWrapperFactory;
using static AInq.Support.Background.WorkFactory;

namespace AInq.Support.Background.Managers
{
    internal sealed class PriorityWorkQueueManager : WorkQueueManager, IPriorityWorkQueue, ITaskManager<object, int>
    {
        private readonly int _maxPriority;
        private readonly IList<ConcurrentQueue<ITaskWrapper<object>>> _queues;

        int IPriorityWorkQueue.MaxPriority => _maxPriority;

        bool ITaskManager<object, int>.HasTask => _queues.Any(queue => !queue.IsEmpty);

        Task ITaskManager<object, int>.WaitForTaskAsync(CancellationToken cancellation)
            => _queues.Any(queue => !queue.IsEmpty)
                ? Task.CompletedTask
                : NewWorkEvent.WaitAsync(cancellation);

        (ITaskWrapper<object>, int) ITaskManager<object, int>.GetTask()
        {
            var pendingQueue = _queues.FirstOrDefault(queue => !queue.IsEmpty);
            return pendingQueue != null && pendingQueue.TryDequeue(out var task)
                ? (task, _queues.IndexOf(pendingQueue))
                : (null, -1);
        }

        void ITaskManager<object, int>.RevertTask(ITaskWrapper<object> task, int metadata)
        {
            if (metadata < 0 || metadata > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(metadata), metadata, null);
            _queues[metadata].Enqueue(task);
            NewWorkEvent.Set();
        }

        internal PriorityWorkQueueManager(int maxPriority)
        {
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            _maxPriority = maxPriority;
            _queues = new ConcurrentQueue<ITaskWrapper<object>>[_maxPriority + 1];
            _queues[0] = Queue;
            for (var index = 1; index <= _maxPriority; index++)
                _queues[index] = new ConcurrentQueue<ITaskWrapper<object>>();
        }

        Task IPriorityWorkQueue.EnqueueWork(IWork work, int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IPriorityWorkQueue.EnqueueWork<TWork>(int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueWork<TWork, TResult>(int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IPriorityWorkQueue.EnqueueAsyncWork(IAsyncWork work, int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IPriorityWorkQueue.EnqueueAsyncWork<TWork>(int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IPriorityWorkQueue.EnqueueAsyncWork<TWork, TResult>(int priority, CancellationToken cancellation, int attemptsCount)
        {
            if (priority < 0 || priority > _maxPriority)
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), attemptsCount, cancellation);
            _queues[priority].Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }
    }
}