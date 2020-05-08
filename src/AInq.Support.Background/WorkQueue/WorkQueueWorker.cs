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
using System.Threading;
using System.Threading.Tasks;
using AInq.Support.Background.WorkElements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;
using Nito.AsyncEx.Interop;

namespace AInq.Support.Background.WorkQueue
{
    internal class WorkQueueWorker : IHostedService, IDisposable
    {
        private readonly WorkQueueManager _workQueueManager;
        private readonly IServiceProvider _provider;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private Task _worker;

        internal WorkQueueWorker(WorkQueueManager workQueueManager, IServiceProvider provider)
        {
            _workQueueManager = workQueueManager;
            _provider = provider;
        }

        protected virtual async Task<bool> GetNextWorkAsync()
        {
            if (!_workQueueManager.Queue.TryDequeue(out var work)) return false;
            await DoWorkAsync(work);
            return !_workQueueManager.Queue.IsEmpty;
        }

        protected async Task DoWorkAsync(IWorkWrapper work)
        {
            using var scope = _provider.CreateScope();
            await work.DoWorkAsync(scope.ServiceProvider, _cancellation.Token);
        }

        private async Task Worker()
        {
            using var cancel = WaitHandleAsyncFactory.FromWaitHandle(_cancellation.Token.WaitHandle);
            while (!_cancellation.IsCancellationRequested)
            {
                while (await GetNextWorkAsync()) { }
                await Task.WhenAny(_workQueueManager.NewWorkEvent.WaitAsync(), cancel);
            }
        }

        Task IHostedService.StartAsync(CancellationToken cancel)
        {
            cancel.ThrowIfCancellationRequested();
            _worker = Worker();
            return Task.CompletedTask;
        }

        async Task IHostedService.StopAsync(CancellationToken cancel)
        {
            _cancellation.Cancel();
            await _worker.WaitAsync(cancel);
        }

        public void Dispose()
        {
            _cancellation.Dispose();
            _worker.Dispose();
        }
    }
}