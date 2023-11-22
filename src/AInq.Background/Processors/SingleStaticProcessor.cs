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

namespace AInq.Background.Processors;

internal sealed class SingleStaticProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>, IDisposable, IAsyncDisposable
{
    private readonly TArgument _argument;

    internal SingleStaticProcessor(TArgument argument)
        => _argument = argument;

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _argument.TryDisposeAsync().ConfigureAwait(false);

    void IDisposable.Dispose()
        => (_argument as IDisposable)?.Dispose();

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider,
        ILogger logger, CancellationToken cancellation)
    {
        _ = manager ?? throw new ArgumentNullException(nameof(manager));
        _ = provider ?? throw new ArgumentNullException(nameof(provider));
        _ = logger ?? throw new ArgumentNullException(nameof(logger));
        if (!manager.HasTask) return;
        try
        {
            if (_argument is IStartStoppable {IsActive: false} startStoppable)
                await startStoppable.ActivateAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting stoppable argument {Argument}", _argument);
            return;
        }
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            if (_argument is IStartStoppable {IsActive: false}) break;
            var (task, metadata) = manager.GetTask();
            if (task == null) continue;
            if (!await task.ExecuteAsync(_argument, provider, logger, cancellation).ConfigureAwait(false))
                manager.RevertTask(task, metadata);
            try
            {
                if (_argument is IThrottling {Timeout.Ticks: > 0} throttling)
                    await Task.Delay(throttling.Timeout, cancellation).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        try
        {
            if (_argument is IStartStoppable {IsActive: true} startStoppable)
                await startStoppable.DeactivateAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error stopping stoppable argument {Argument}", _argument);
        }
    }
}
