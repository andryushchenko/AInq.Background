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

using AInq.Support.Background.Managers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Processors
{
    internal sealed class MultipleNullTaskProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata> where TArgument:class
    {
        private readonly SemaphoreSlim _semaphore;

        public MultipleNullTaskProcessor(int maxSimultaneousTasks)
        {
            if (maxSimultaneousTasks < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneousTasks), maxSimultaneousTasks, null);
            _semaphore = new SemaphoreSlim(maxSimultaneousTasks);
        }

        async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskQueueManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
        {
            if (!manager.HasTask)
                return;
            while (manager.HasTask)
            {
                await _semaphore.WaitAsync(cancellation);
                var (task, metadata) = manager.GetTask();
                if (task == null)
                {
                    _semaphore.Release();
                    break;
                }
                _ = Task.Run(async () =>
                {
                    if (!await task.ExecuteAsync(null, provider, cancellation))
                        manager.RevertTask(task, metadata);
                    _semaphore.Release();
                }, cancellation);
            }
        }
    }
}