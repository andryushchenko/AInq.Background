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

using System.Threading;
using System.Threading.Tasks;
using AInq.Support.Background.WorkElements;

namespace AInq.Support.Background.WorkQueue

{
    public interface IPriorityWorkQueue : IWorkQueue
    {
        int MaxPriority { get; }
        Task EnqueueWork(IWork work, int priority, CancellationToken cancellation = default);
        Task EnqueueWork<TWork>(int priority, CancellationToken cancellation = default) where TWork : IWork;
        Task<TResult> EnqueueWork<TResult>(IWork<TResult> work, int priority, CancellationToken cancellation = default);
        Task<TResult> EnqueueWork<TWork, TResult>(int priority, CancellationToken cancellation = default) where TWork : IWork<TResult>;
        Task EnqueueAsyncWork(IAsyncWork work, int priority, CancellationToken cancellation = default);
        Task EnqueueAsyncWork<TWork>(int priority, CancellationToken cancellation = default) where TWork : IAsyncWork;
        Task<TResult> EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, int priority, CancellationToken cancellation = default);
        Task<TResult> EnqueueAsyncWork<TWork, TResult>(int priority, CancellationToken cancellation = default) where TWork : IAsyncWork<TResult>;
    }
}