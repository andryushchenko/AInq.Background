// Copyright 2020-2022 Anton Andryushchenko
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

namespace AInq.Background.Processors;

internal class MultipleOneTimeProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
{
    private readonly Func<IServiceProvider, TArgument> _argumentFabric;
    private readonly SemaphoreSlim _semaphore;

    internal MultipleOneTimeProcessor(Func<IServiceProvider, TArgument> argumentFabric, int maxParallelTasks)
    {
        _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        _semaphore = new SemaphoreSlim(Math.Max(1, maxParallelTasks));
    }

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider,
        ILogger logger, CancellationToken cancellation)
    {
        _ = manager ?? throw new ArgumentNullException(nameof(manager));
        _ = provider ?? throw new ArgumentNullException(nameof(provider));
        _ = logger ?? throw new ArgumentNullException(nameof(logger));
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            var (task, metadata) = manager.GetTask();
            if (task == null) continue;
            await _semaphore.WaitAsync(cancellation).ConfigureAwait(false);
            Execute(task, metadata, manager, provider, logger, cancellation);
        }
    }

    private async void Execute(ITaskWrapper<TArgument> task, TMetadata metadata, ITaskManager<TArgument, TMetadata> manager,
        IServiceProvider provider, ILogger logger, CancellationToken cancellation)
    {
        TArgument argument;
        try
        {
            argument = _argumentFabric.Invoke(provider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating argument {Type} with {Fabric}", typeof(TArgument), _argumentFabric);
            manager.RevertTask(task, metadata);
            _semaphore.Release();
            return;
        }
        try
        {
            if (argument is IStartStoppable {IsActive: false} startStoppable)
                await startStoppable.ActivateAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting stoppable argument {Argument}", argument);
            manager.RevertTask(task, metadata);
            _semaphore.Release();
            return;
        }
        if (!await task.ExecuteAsync(argument, provider, logger, cancellation).ConfigureAwait(false))
            manager.RevertTask(task, metadata);
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
            _semaphore.Release();
            (argument as IDisposable)?.Dispose();
        }
    }
}
