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

using AInq.Background.Helpers;
using AInq.Background.Managers;
using AInq.Background.Wrappers;

namespace AInq.Background.Processors;

internal sealed class MultipleStaticProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>, IDisposable, IAsyncDisposable
{
    private readonly ConcurrentBag<TArgument> _active = new();
    private readonly ConcurrentBag<TArgument> _inactive;
    private readonly AsyncAutoResetEvent _reset = new(false);

    internal MultipleStaticProcessor(IEnumerable<TArgument> arguments)
    {
        _inactive = new ConcurrentBag<TArgument>(arguments);
        if (_inactive.IsEmpty) throw new ArgumentException("Empty collection", nameof(arguments));
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        while (_active.TryTake(out var argument) || _inactive.TryTake(out argument))
            await argument.TryDisposeAsync().ConfigureAwait(false);
    }

    void IDisposable.Dispose()
    {
        while (_active.TryTake(out var argument) || _inactive.TryTake(out argument))
            (argument as IDisposable)?.Dispose();
    }

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider,
        ILogger logger, CancellationToken cancellation)
    {
        _ = manager ?? throw new ArgumentNullException(nameof(manager));
        _ = provider ?? throw new ArgumentNullException(nameof(provider));
        _ = logger ?? throw new ArgumentNullException(nameof(logger));
        var currentTasks = new LinkedList<Task>();
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            if (!_active.TryTake(out var argument) && !_inactive.TryTake(out argument))
            {
                if (_active.IsEmpty && _inactive.IsEmpty)
                    await _reset.WaitAsync(cancellation).ConfigureAwait(false);
                continue;
            }
            var (task, metadata) = manager.GetTask();
            if (task == null)
            {
                if (argument is IStartStoppable {IsActive: true})
                    _active.Add(argument);
                else _inactive.Add(argument);
                _reset.Set();
                continue;
            }
            currentTasks.AddLast(Execute(argument, task, metadata, manager, provider, logger, cancellation));
        }
        UtilizeArguments(Task.WhenAll(currentTasks), logger, cancellation);
    }

    private async Task Execute(TArgument argument, ITaskWrapper<TArgument> task, TMetadata metadata, ITaskManager<TArgument, TMetadata> manager,
        IServiceProvider provider, ILogger logger, CancellationToken cancellation)
    {
        try
        {
            if (argument is IStartStoppable {IsActive: false} startStoppable)
                await startStoppable.ActivateAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting stoppable argument {Argument}", argument);
            manager.RevertTask(task, metadata);
            _inactive.Add(argument);
            _reset.Set();
            return;
        }
        if (!await task.ExecuteAsync(argument, provider, logger, cancellation).ConfigureAwait(false))
            manager.RevertTask(task, metadata);
        try
        {
            if (argument is IThrottling {Timeout.Ticks: > 0} throttling)
                await Task.Delay(throttling.Timeout, cancellation).ConfigureAwait(false);
        }
        finally
        {
            if (argument is IStartStoppable {IsActive: true})
                _active.Add(argument);
            else _inactive.Add(argument);
            _reset.Set();
        }
    }

    private async void UtilizeArguments(Task finishTask, ILogger logger, CancellationToken cancellation)
    {
        await finishTask.ConfigureAwait(false);
        while (!_active.IsEmpty)
            if (_active.TryTake(out var argument))
                UtilizeArgument(argument, logger, cancellation);
    }

    private async void UtilizeArgument(TArgument argument, ILogger logger, CancellationToken cancellation)
    {
        try
        {
            if (argument is IStartStoppable {IsActive: true} startStoppable)
                await startStoppable.DeactivateAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error stopping stoppable argument {Argument}", argument);
        }
        finally
        {
            _inactive.Add(argument);
            _reset.Set();
        }
    }
}
