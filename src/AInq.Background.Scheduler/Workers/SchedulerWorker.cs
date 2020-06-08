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

using AInq.Background.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Workers
{

internal sealed class SchedulerWorker : IHostedService, IDisposable
{
    private readonly WorkSchedulerManager _scheduler;
    private readonly IServiceProvider _provider;
    private readonly ILogger<SchedulerWorker>? _logger;
    private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
    private Task? _worker;

    public SchedulerWorker(WorkSchedulerManager scheduler, IServiceProvider provider)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _provider = provider;
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<SchedulerWorker>();
    }

    private async Task Worker(CancellationToken abort)
    {
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_shutdown.Token, abort);
        while (!cancellation.IsCancellationRequested)
            try
            {
                // TODO
            }
            catch (OperationCanceledException)
            {
                return;
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
