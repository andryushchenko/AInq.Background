// Copyright 2020-2023 Anton Andryushchenko
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
using AInq.Background.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Nito.AsyncEx;

namespace AInq.Background.Workers;

/// <summary> Background scheduled task worker service </summary>
public sealed class SchedulerWorker : IHostedService, IDisposable
{
    private static readonly TimeSpan DefaultHorizon = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan MinHorizon = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan MaxTimeout = TimeSpan.FromHours(1);
    private static readonly TimeSpan Beforehand = TimeSpan.FromSeconds(5);

    private readonly TimeSpan _horizon;
    private readonly ILogger<SchedulerWorker> _logger;
    private readonly IServiceProvider _provider;
    private readonly IWorkSchedulerManager _scheduler;
    private readonly CancellationTokenSource _shutdown = new();

    private Task? _worker;

    /// <param name="scheduler"> Work scheduler manager instance </param>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="horizon"> Upcoming task search horizon </param>
    public SchedulerWorker(IWorkSchedulerManager scheduler, IServiceProvider provider, TimeSpan? horizon = null)
    {
        if (horizon < MinHorizon)
            horizon = MinHorizon;
        if (horizon > MaxTimeout)
            horizon = MaxTimeout;
        _horizon = horizon ?? DefaultHorizon;
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _logger = provider.GetService<ILogger<SchedulerWorker>>() ?? NullLogger<SchedulerWorker>.Instance;
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
                var pending = _scheduler.GetUpcomingTasks(_horizon + Beforehand);
                foreach (var group in pending)
                foreach (var work in group)
                    ProcessWork(work, group.Key.ToLocalTime() - DateTime.Now, cancellation.Token).Ignore();
                var timeout = _scheduler.GetNextTaskTime()?.ToLocalTime().Subtract(Beforehand).Subtract(DateTime.Now) ?? TimeSpan.MaxValue;
                if (timeout < Beforehand)
                    continue;
                await Task.WhenAny(Task.Delay(timeout < MaxTimeout ? timeout : MaxTimeout, cancellation.Token),
                              _scheduler.WaitForNewTaskAsync(cancellation.Token))
                          .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error processing scheduled tasks");
            }
    }

    private async Task ProcessWork(IScheduledTaskWrapper work, TimeSpan delay, CancellationToken cancellation)
    {
        if (work.IsCanceled) return;
        try
        {
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _scheduler.RevertTask(work);
            return;
        }
        if (work.IsCanceled) return;
        if (await work.ExecuteAsync(_provider, _logger, cancellation).ConfigureAwait(false))
            _scheduler.RevertTask(work);
    }
}
