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

using AInq.Background.Helpers;
using AInq.Background.Managers;
using AInq.Background.Wrappers;
using DotNext.Threading;

namespace AInq.Background.Processors;

internal sealed class MultipleReusableProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
{
    private readonly Func<IServiceProvider, TArgument> _argumentFabric;
    private readonly int _maxArgumentCount;
    private readonly AsyncAutoResetEvent _reset = new(false);
    private readonly ConcurrentBag<TArgument> _reusable = [];
    private int _currentArgumentCount;

    internal MultipleReusableProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxArgumentsCount)
    {
        _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        _maxArgumentCount = Math.Max(1, maxArgumentsCount);
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
            if (!_reusable.TryTake(out var argument))
            {
                if (_currentArgumentCount < _maxArgumentCount)
                {
                    try
                    {
                        argument = _argumentFabric.Invoke(provider);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating argument {Type} with {Fabric}", typeof(TArgument), _argumentFabric);
                        continue;
                    }
                    Interlocked.Increment(ref _currentArgumentCount);
                }
                else
                {
                    if (_reusable.IsEmpty)
#if NETSTANDARD
                        await _reset.Wait(cancellation).ConfigureAwait(false);
#else
                        await _reset.WaitAsync(cancellation).ConfigureAwait(false);
#endif
                    continue;
                }
            }
            var (task, metadata) = manager.GetTask();
            if (task == null)
            {
                _reusable.Add(argument);
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
            Interlocked.Decrement(ref _currentArgumentCount);
            _reset.Set();
            await argument.TryDisposeAsync().ConfigureAwait(false);
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
            _reusable.Add(argument);
            _reset.Set();
        }
    }

    private async void UtilizeArguments(Task finishTask, ILogger logger, CancellationToken cancellation)
    {
        await finishTask.ConfigureAwait(false);
        while (!_reusable.IsEmpty)
            if (_reusable.TryTake(out var argument))
            {
                Interlocked.Decrement(ref _currentArgumentCount);
                _reset.Set();
                UtilizeArgument(argument, logger, cancellation);
            }
    }

    private static async void UtilizeArgument(TArgument argument, ILogger logger, CancellationToken cancellation)
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
            await argument.TryDisposeAsync().ConfigureAwait(false);
        }
    }
}
