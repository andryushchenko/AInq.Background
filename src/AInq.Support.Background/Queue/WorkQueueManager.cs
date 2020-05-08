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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace AInq.Support.Background.Queue
{
    internal class WorkQueueManager : IWorkQueue
    {
        protected internal ConcurrentQueue<WorkWrapper.IWorkWrapper> Queue { get; } = new ConcurrentQueue<WorkWrapper.IWorkWrapper>();
        protected internal AsyncAutoResetEvent NewWorkEvent { get; } = new AsyncAutoResetEvent(false);

        Task IWorkQueue.EnqueueWork(IWork work, CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IWorkQueue.EnqueueWork<TWork>(CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(WorkFactory.CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueWork<TResult>(IWork<TResult> work, CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueWork<TWork, TResult>(CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(WorkFactory.CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IWorkQueue.EnqueueAsyncWork(IAsyncWork work, CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task IWorkQueue.EnqueueAsyncWork<TWork>(CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(WorkFactory.CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }

        Task<TResult> IWorkQueue.EnqueueAsyncWork<TWork, TResult>(CancellationToken cancellation)
        {
            var (workWrapper, task) = WorkWrapper.CreateWorkWrapper(WorkFactory.CreateWork((provider, token) => provider.GetRequiredService<TWork>().DoWorkAsync(provider, token)), cancellation);
            Queue.Enqueue(workWrapper);
            NewWorkEvent.Set();
            return task;
        }
    }
}