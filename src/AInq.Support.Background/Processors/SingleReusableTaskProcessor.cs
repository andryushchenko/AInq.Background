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

using AInq.Support.Background.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Processors
{
    internal sealed class SingleReusableTaskProcessor<TArgument, TMetadata> : ITaskProcessor<TArgument, TMetadata>
    {
        private readonly Func<IServiceProvider, TArgument> _argumentFabric;

        internal SingleReusableTaskProcessor(Func<IServiceProvider, TArgument> argumentFabric)
        {
            _argumentFabric = argumentFabric ?? throw new ArgumentNullException(nameof(argumentFabric));
        }

        async Task ITaskProcessor<TArgument, TMetadata>.ProcessPendingTasksAsync(ITaskQueueManager<TArgument, TMetadata> manager, IServiceProvider provider, CancellationToken cancellation)
        {
            if (!manager.HasTask)
                return;
            var argument = _argumentFabric.Invoke(provider);
            var machine = argument as IStoppableTaskMachine;
            if (machine != null && !machine.IsRunning)
                await machine.StartMachineAsync(cancellation);
            while (manager.HasTask)
            {
                var (task, metadata) = manager.GetTask();
                if (task == null) break;
                using var taskScope = provider.CreateScope();
                if (!await task.ExecuteAsync(argument, taskScope.ServiceProvider, cancellation))
                    manager.RevertTask(task, metadata);
                if (manager.HasTask && argument is IThrottlingTaskMachine throttling && throttling.Timeout.Ticks > 0)
                    await Task.Delay(throttling.Timeout, cancellation);
            }
            if (machine != null && machine.IsRunning)
                await machine.StopMachineAsync(cancellation);
        }
    }
}