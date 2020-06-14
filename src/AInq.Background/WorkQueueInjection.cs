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

using AInq.Background.Managers;
using AInq.Background.Processors;
using AInq.Background.Workers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AInq.Background
{

public static class WorkQueueInjection
{
    public static IServiceCollection AddWorkQueue(this IServiceCollection services, int maxSimultaneous = 1, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IWorkQueue)))
            throw new InvalidOperationException("Service already exists");
        var manager = new WorkQueueManager(maxAttempts);
        services.AddSingleton<IWorkQueue>(manager);
        return maxSimultaneous <= 1
            ? services.AddHostedService(provider => new TaskWorker<object?, object?>(provider, manager, new SingleNullProcessor<object?>()))
            : services.AddHostedService(provider => new TaskWorker<object?, object?>(provider, manager, new MultipleNullProcessor<object?>(maxSimultaneous)));
    }

    public static IServiceCollection AddPriorityWorkQueue(this IServiceCollection services, int maxPriority = 100, int maxSimultaneous = 1, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IWorkQueue)))
            throw new InvalidOperationException("Service already exists");
        var manager = new PriorityWorkQueueManager(maxPriority, maxAttempts);
        services.AddSingleton<IWorkQueue>(manager)
                .AddSingleton<IPriorityWorkQueue>(manager);
        return maxSimultaneous <= 1
            ? services.AddHostedService(provider => new TaskWorker<object?, int>(provider, manager, new SingleNullProcessor<int>()))
            : services.AddHostedService(provider => new TaskWorker<object?, int>(provider, manager, new MultipleNullProcessor<int>(maxSimultaneous)));
    }
}

}
