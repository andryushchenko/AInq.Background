// Copyright 2020-2021 Anton Andryushchenko
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
using AInq.Background.Services;
using AInq.Background.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace AInq.Background;

/// <summary> Work Scheduler dependency injection </summary>
public static class WorkSchedulerInjection
{
    /// <summary> Create <see cref="IWorkScheduler" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="horizon"> Time horizon to look for upcoming tasks </param>
    public static IWorkScheduler CreateWorkScheduler(this IServiceCollection services, TimeSpan? horizon = null)
    {
        var scheduler = new WorkSchedulerManager();
        services.AddHostedService(provider => new SchedulerWorker(scheduler, provider, horizon));
        return scheduler;
    }

    /// <summary> Add <see cref="IWorkScheduler" /> service </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="horizon"> Time horizon to look for upcoming tasks </param>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    public static IServiceCollection AddWorkScheduler(this IServiceCollection services, TimeSpan? horizon = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(IWorkScheduler)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateWorkScheduler(horizon));
    }
}
