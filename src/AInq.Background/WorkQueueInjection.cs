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
using AInq.Background.Processors;
using AInq.Background.Services;
using AInq.Background.Workers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AInq.Background
{

/// <summary> Background Work Queue dependency injection </summary>
public static class WorkQueueInjection
{
    /// <summary> Create <see cref="IWorkQueue" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="maxParallelWorks"> Max parallel works allowed </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public static IWorkQueue CreateWorkQueue(this IServiceCollection services, int maxParallelWorks = 1, int maxAttempts = int.MaxValue)
    {
        var manager = new WorkQueueManager(maxAttempts);
        services.AddHostedService(provider
            => new TaskWorker<object?, object?>(provider, manager, ProcessorFactory.CreateNullProcessor<object?>(maxParallelWorks)));
        return manager;
    }

    /// <summary> Add <see cref="IWorkQueue" /> service </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="maxParallelWorks"> Max parallel works allowed </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    public static IServiceCollection AddWorkQueue(this IServiceCollection services, int maxParallelWorks = 1, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IWorkQueue)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateWorkQueue(maxParallelWorks, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityWorkQueue" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="maxPriority"> Max allowed work priority </param>
    /// <param name="maxParallelWorks"> Max allowed parallel works </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public static IPriorityWorkQueue CreatePriorityWorkQueue(this IServiceCollection services, int maxPriority = 100, int maxParallelWorks = 1,
        int maxAttempts = int.MaxValue)
    {
        var manager = new PriorityWorkQueueManager(maxPriority, maxAttempts);
        services.AddHostedService(provider
            => new TaskWorker<object?, int>(provider, manager, ProcessorFactory.CreateNullProcessor<int>(maxParallelWorks)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityWorkQueue" /> and <see cref="IWorkQueue" /> services </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="maxPriority"> Max allowed work priority </param>
    /// <param name="maxParallelWorks"> Max allowed parallel works </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    public static IServiceCollection AddPriorityWorkQueue(this IServiceCollection services, int maxPriority = 100, int maxParallelWorks = 1,
        int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IWorkQueue)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityWorkQueue(maxPriority, maxParallelWorks, maxAttempts);
        return services.AddSingleton<IWorkQueue>(queue).AddSingleton(queue);
    }
}

}
