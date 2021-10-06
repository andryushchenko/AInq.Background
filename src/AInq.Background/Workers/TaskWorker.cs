// Copyright 2020-2021 Anton Andryushchenko
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
        _provider = provider;
        _manager = manager;
        _processor = processor;
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<TaskWorker<TArgument, TMetadata>>()
                  ?? NullLogger<TaskWorker<TArgument, TMetadata>>.Instance;
    }

    void IDisposable.Dispose()
    {
        _shutdown.Dispose();
        if (_worker != null && (_worker.IsCompleted || _worker.IsFaulted || _worker.IsCanceled))
            _worker.Dispose();
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
            await _worker.WaitAsync(cancel).ConfigureAwait(false);
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
    }
}
