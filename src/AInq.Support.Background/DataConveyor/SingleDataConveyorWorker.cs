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

using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.DataConveyor
{
    internal class SingleDataConveyorWorker<TData, TResult> : IHostedService, IDisposable
    {
        private readonly DataConveyorManager<TData, TResult> _conveyorManager;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly IDataConveyorMachine<TData, TResult> _machine;
        private Task _worker;

        internal SingleDataConveyorWorker(DataConveyorManager<TData, TResult> conveyorManager, IDataConveyorMachine<TData, TResult> machine)
        {
            _conveyorManager = conveyorManager ?? throw new ArgumentNullException(nameof(conveyorManager));
            _machine = machine ?? throw new ArgumentNullException(nameof(machine));
        }

        protected virtual async Task<bool> ProcessNextElementAsync()
        {
            if (!_conveyorManager.Queue.TryDequeue(out var element)) return false;
            if (await ProcessElementAsync(element)) return !_conveyorManager.Queue.IsEmpty;
            _conveyorManager.Queue.Enqueue(element);
            return true;
        }

        protected async Task<bool> ProcessElementAsync(DataConveyorElement<TData, TResult> element)
        {
            return await element.ProcessDataAsync(_machine, _cancellation.Token);
        }

        private async Task Worker()
        {
            while (!_cancellation.IsCancellationRequested)
                try
                {
                    await _conveyorManager.NewDataEvent.WaitAsync(_cancellation.Token);
                    await _machine.StartConveyorAsync(_cancellation.Token);
                    while (await ProcessNextElementAsync())
                        if (_machine.Timeout.HasValue)
                            await Task.Delay(_machine.Timeout.Value);
                    await _machine.StopConveyorAsync(_cancellation.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
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