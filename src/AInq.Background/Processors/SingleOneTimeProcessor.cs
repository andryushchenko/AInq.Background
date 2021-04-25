// Copyright 2021 Anton Andryushchenko
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
using AInq.Background.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Processors
{

internal sealed class SingleOneTimeProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
{
    private readonly Func<IServiceProvider, TArgument> _argumentFabric;

    internal SingleOneTimeProcessor(Func<IServiceProvider, TArgument> argumentFabric)
        => _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));

    async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider,
        ILogger? logger, CancellationToken cancellation)
    {
        while (manager.HasTask && !cancellation.IsCancellationRequested)
        {
            var (task, metadata) = manager.GetTask();
            if (task == null)
                continue;
            using var taskScope = provider.CreateScope();
            TArgument argument;
            try
            {
                argument = _argumentFabric.Invoke(taskScope.ServiceProvider);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error creating argument {Type} with {Fabric}", typeof(TArgument), _argumentFabric);
                manager.RevertTask(task, metadata);
                continue;
            }
            var startStoppable = argument as IStartStoppable;
            try
            {
                if (startStoppable != null && !startStoppable.IsActive)
                    await startStoppable.ActivateAsync(cancellation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error starting stoppable argument {Argument}", startStoppable);
                manager.RevertTask(task, metadata);
                continue;
            }
            if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, logger, cancellation).ConfigureAwait(false))
                manager.RevertTask(task, metadata);
            Task.Run(async () =>
                    {
                        try
                        {
                            if (startStoppable != null && startStoppable.IsActive)
                                await startStoppable.DeactivateAsync(cancellation).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error stopping stoppable argument {Argument}", startStoppable);
                        }
                    },
                    cancellation)
                .Ignore();
        }
    }
}

}
