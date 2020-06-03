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
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Support.Background.Elements.WorkWrapperFactory;
using static AInq.Support.Background.WorkFactory;

namespace AInq.Support.Background.Managers
{
    internal class WorkQueueManager : IWorkQueue, ITaskManager<object, object>
    {
        protected readonly ConcurrentQueue<ITaskWrapper<object>> Queue = new ConcurrentQueue<ITaskWrapper<object>>();
        protected readonly AsyncAutoResetEvent NewWorkEvent = new AsyncAutoResetEvent(false);

        bool ITaskManager<object, object>.HasTask => !Queue.IsEmpty;

        Task ITaskManager<object, object>.WaitForTaskAsync(CancellationToken cancellation)
            => Queue.IsEmpty
                ? NewWorkEvent.WaitAsync(cancellation)
                : Task.CompletedTask;

        (ITaskWrapper<object>, object) ITaskManager<object, object>.GetTask()
            => (Queue.TryDequeue(out var task)
                ? task
                : null, null);

        void ITaskManager<object, object>.RevertTask(ITaskWrapper<object> task, object metadata)
            => Queue.Enqueue(task);

        Task IWorkQueue.EnqueueWork(IWork work, CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IWorkQueue.EnqueueWork<TWork>(CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueWork<TWork, TResult>(CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IWorkQueue.EnqueueAsyncWork(IAsyncWork work, CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IWorkQueue.EnqueueAsyncWork<TWork>(CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueAsyncWork<TWork, TResult>(CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var (workWrapper, task) = CreateWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), attemptsCount, cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }
    }
}