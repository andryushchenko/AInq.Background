﻿// Copyright 2020 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using AInq.Background.Managers;
using AInq.Background.Processors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
#if NETSTANDARD
using DotNext.Threading.Tasks;
#endif

namespace AInq.Background.Workers;

/// <summary> Background task worker service </summary>
/// <typeparam name="TArgument"> Task argument type </typeparam>
/// <typeparam name="TMetadata"> Task metadata type </typeparam>
public sealed class TaskWorker<TArgument, TMetadata> : IHostedService, IDisposable
{
    private readonly ILogger<TaskWorker<TArgument, TMetadata>> _logger;
    private readonly ITaskManager<TArgument, TMetadata> _manager;
    private readonly ITaskProcessor<TArgument, TMetadata> _processor;
    private readonly IServiceProvider _provider;
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _worker;

    /// <param name="provider"> Service provider instance </param>
    /// <param name="manager"> Task manager instance </param>
    /// <param name="processor"> Task processor instance </param>
    public TaskWorker(IServiceProvider provider, ITaskManager<TArgument, TMetadata> manager, ITaskProcessor<TArgument, TMetadata> processor)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _logger = provider.GetService<ILogger<TaskWorker<TArgument, TMetadata>>>() ?? NullLogger<TaskWorker<TArgument, TMetadata>>.Instance;
    }

    void IDisposable.Dispose()
    {
        _shutdown.Dispose();
        (_processor as IDisposable)?.Dispose();
    }

    Task IHostedService.StartAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        _worker = Worker(cancel);
        return Task.CompletedTask;
    }

    async Task IHostedService.StopAsync(CancellationToken cancel)
    {
#if NET8_0_OR_GREATER
        await _shutdown.CancelAsync().ConfigureAwait(false);
#else
        _shutdown.Cancel();
#endif
        if (_worker != null)
#if NETSTANDARD
            await _worker.WaitAsync(TimeSpan.MaxValue, cancel).ConfigureAwait(false);
#else
            await _worker.WaitAsync(cancel).ConfigureAwait(false);
#endif
    }

    private async Task Worker(CancellationToken abort)
    {
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_shutdown.Token, abort);
        while (!cancellation.IsCancellationRequested)
            try
            {
                while (_manager.HasTask && !cancellation.IsCancellationRequested)
                    await _processor.ProcessPendingTasksAsync(_manager, _provider, _logger, cancellation.Token).ConfigureAwait(false);
                await _manager.WaitForTaskAsync(cancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in task processor {ProcessorType}", _processor.GetType());
            }
    }
}
