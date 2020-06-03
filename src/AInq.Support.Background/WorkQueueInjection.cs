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
using AInq.Support.Background.Processors;
using AInq.Support.Background.Workers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AInq.Support.Background
{
    public static class WorkQueueInjection
    {
        public static IServiceCollection AddWorkQueue(this IServiceCollection services, int maxSimultaneous = 1)
        {
            if (services.Any(service => service.ImplementationType == typeof(IWorkQueue)))
                throw new InvalidOperationException("Service already exists");
            if (maxSimultaneous < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
            var manager = new WorkQueueManager();
            services.AddSingleton<IWorkQueue>(manager);
            return maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<object, object>(provider, manager, new SingleNullTaskProcessor<object, object>()))
                : services.AddHostedService(provider => new TaskWorker<object, object>(provider, manager, new MultipleNullTaskProcessor<object, object>(maxSimultaneous)));
        }

        public static IServiceCollection AddPriorityWorkQueue(this IServiceCollection services, int maxPriority, int maxSimultaneous = 1)
        {
            if (services.Any(service => service.ImplementationType == typeof(IWorkQueue)))
                throw new InvalidOperationException("Service already exists");
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            if (maxSimultaneous < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
            var manager = new PriorityWorkQueueManager(maxPriority);
            services.AddSingleton<IWorkQueue>(manager)
                .AddSingleton<IPriorityWorkQueue>(manager);
            return maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<object, int>(provider, manager, new SingleNullTaskProcessor<object, int>()))
                : services.AddHostedService(provider => new TaskWorker<object, int>(provider, manager, new MultipleNullTaskProcessor<object, int>(maxSimultaneous)));
        }
    }
}