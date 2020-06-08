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
using AInq.Background.Managers;
using AInq.Background.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace AInq.Background.Workers
{

internal sealed class TaskWorker<TArgument, TMetadata> : IHostedService, IDisposable
{
    private readonly IServiceProvider _provider;
    private readonly ITaskManager<TArgument, TMetadata> _manager;
    private readonly ITaskProcessor<TArgument, TMetadata> _processor;
    private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
    private readonly ILogger<TaskWorker<TArgument, TMetadata>>? _logger;
    private Task? _worker;

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
                while (_manager.HasTask)
                {
                    using var scope = _provider.CreateScope();
                    await _processor.ProcessPendingTasksAsync(_manager, scope.ServiceProvider, _logger, cancellation.Token);
                }
                await _manager.WaitForTaskAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in task processor {0}", _processor.GetType());
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
        if (_worker != null)
            await _worker.WaitAsync(cancel);
    }

    void IDisposable.Dispose()
    {
        _shutdown.Dispose();
        if (_worker != null && (_worker.IsCompleted || _worker.IsFaulted || _worker.IsCanceled))
            _worker.Dispose();
    }
}

}
