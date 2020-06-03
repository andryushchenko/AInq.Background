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
using AInq.Support.Background.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AInq.Support.Background.Workers
{
    internal sealed class TaskQueueWorker<TArgument, TMetadata> : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly ITaskQueueManager<TArgument, TMetadata> _manager;
        private readonly ITaskProcessor<TArgument, TMetadata> _processor;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly ILogger<TaskQueueWorker<TArgument, TMetadata>> _logger;
        private Task _worker;

        internal TaskQueueWorker(IServiceProvider provider, ITaskQueueManager<TArgument, TMetadata> manager, ITaskProcessor<TArgument, TMetadata> processor)
        {
            _provider = provider;
            _manager = manager;
            _processor = processor;
            _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<TaskQueueWorker<TArgument, TMetadata>>();
        }

        private async Task Worker()
        {
            while (!_cancellation.IsCancellationRequested)
                try
                {
                    await _manager.WaitForTaskAsync(_cancellation.Token);
                    while (_manager.HasTask)
                    {
                        using var scope = _provider.CreateScope();
                        await _processor.ProcessPendingTasksAsync(_manager, scope.ServiceProvider, _cancellation.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in Task Processor");
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