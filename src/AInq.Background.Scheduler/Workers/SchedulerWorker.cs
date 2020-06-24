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
using AInq.Background.Wrappers;
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
    private static readonly TimeSpan DefaultHorizon = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan MinHorizon = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan MaxTimeout = TimeSpan.FromHours(1);
    private static readonly TimeSpan Beforehand = TimeSpan.FromSeconds(5);

    private readonly TimeSpan _horizon;
    private readonly ILogger<SchedulerWorker>? _logger;
    private readonly IServiceProvider _provider;
    private readonly IWorkSchedulerManager _scheduler;
    private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();

    private Task? _worker;

    public SchedulerWorker(IWorkSchedulerManager scheduler, IServiceProvider provider, TimeSpan? horizon = null)
    {
        if (horizon.HasValue && horizon < MinHorizon)
            horizon = MinHorizon;
        if (horizon.HasValue && horizon > MaxTimeout)
            horizon = MaxTimeout;
        _horizon = horizon ?? DefaultHorizon;
        _scheduler = scheduler;
        _provider = provider;
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<SchedulerWorker>();
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
                await Task.WhenAny(Task.Delay(timeout < MaxTimeout
                                      ? timeout
                                      : MaxTimeout,
                                  cancellation.Token),
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
        if (work.IsCanceled)
            return;
        try
        {
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _scheduler.RevertWork(work);
            return;
        }
        if (work.IsCanceled)
            return;
        using var scope = _provider.CreateScope();
        if (await work.ExecuteAsync(scope.ServiceProvider, _logger, cancellation).ConfigureAwait(false))
            _scheduler.RevertWork(work);
    }
}

}
