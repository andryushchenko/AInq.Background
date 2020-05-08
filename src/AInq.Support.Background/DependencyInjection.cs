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

using System.Linq;
using AInq.Support.Background.Queue;
using Microsoft.Extensions.DependencyInjection;

namespace AInq.Support.Background
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWorkQueue(this IServiceCollection services)
        {
            if (services.Any(service => service.ServiceType == typeof(IWorkQueue))) return services;
            var queue = new WorkQueueManager();
            return services.AddSingleton<IWorkQueue>(queue)
                .AddHostedService(provider => new WorkQueueWorker(queue, provider));
        }

        public static IServiceCollection AddPriorityWorkQueue(this IServiceCollection services, int maxPriority)
        {
            if (services.Any(service => service.ServiceType == typeof(IWorkQueue))) return services;
            var queue = new PriorityWorkQueueManager(maxPriority);
            return services.AddSingleton<IWorkQueue>(queue)
                .AddSingleton<IPriorityWorkQueue>(queue)
                .AddHostedService(provider => new PriorityWorkQueueWorker(queue, provider));
        }
    }
}