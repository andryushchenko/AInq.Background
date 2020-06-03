﻿/*
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
    internal sealed class TaskWorker<TArgument, TMetadata> : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly ITaskManager<TArgument, TMetadata> _manager;
        private readonly ITaskProcessor<TArgument, TMetadata> _processor;
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private readonly ILogger<TaskWorker<TArgument, TMetadata>> _logger;
        private Task _worker;

        internal TaskWorker(IServiceProvider provider, ITaskManager<TArgument, TMetadata> manager, ITaskProcessor<TArgument, TMetadata> processor)
        {
            _provider = provider;
            _manager = manager;
            _processor = processor;
            _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<TaskWorker<TArgument, TMetadata>>();
        }

        private async Task Worker(CancellationToken abort)
        {
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_shutdown.Token, abort);
            while (!cancellation.IsCancellationRequested)
                try
                {
                    await _manager.WaitForTaskAsync(cancellation.Token);
                    while (_manager.HasTask)
                    {
                        using var scope = _provider.CreateScope();
                        await _processor.ProcessPendingTasksAsync(_manager, scope.ServiceProvider, cancellation.Token);
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
            _worker = Worker(cancel);
            return Task.CompletedTask;
        }

        async Task IHostedService.StopAsync(CancellationToken cancel)
        {
            _shutdown.Cancel();
            await _worker.WaitAsync(cancel);
        }

        public void Dispose()
        {
            _shutdown.Dispose();
            _worker.Dispose();
        }
    }
}