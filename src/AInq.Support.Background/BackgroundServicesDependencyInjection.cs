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

using AInq.Support.Background.DataConveyor;
using AInq.Support.Background.WorkQueue;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AInq.Support.Background
{
    public static class BackgroundServicesDependencyInjection
    {
        public static IServiceCollection AddWorkQueue(this IServiceCollection services)
        {
            if (services.Any(service => service.ServiceType == typeof(IWorkQueue)))
                throw new InvalidOperationException("Service already registered");
            var queue = new WorkQueueManager();
            return services.AddSingleton<IWorkQueue>(queue)
                .AddHostedService(provider => new WorkQueueWorker(queue, provider));
        }

        public static IServiceCollection AddPriorityWorkQueue(this IServiceCollection services, int maxPriority)
        {
            if (services.Any(service => service.ServiceType == typeof(IWorkQueue) || service.ServiceType == typeof(IPriorityWorkQueue)))
                throw new InvalidOperationException("Service already registered");
            var queue = new PriorityWorkQueueManager(maxPriority);
            return services.AddSingleton<IWorkQueue>(queue)
                .AddSingleton<IPriorityWorkQueue>(queue)
                .AddHostedService(provider => new PriorityWorkQueueWorker(queue, provider));
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, IDataConveyorMachine<TData, TResult> conveyorMachine)
        {
            if (services.Any(service => service.ServiceType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already registered");
            var conveyorManager = new DataConveyorManager<TData, TResult>();
            return services.AddSingleton<IDataConveyor<TData, TResult>>(conveyorManager)
                .AddHostedService(provider => new SingleDataConveyorWorker<TData, TResult>(conveyorManager, conveyorMachine));
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IDataConveyorMachine<TData, TResult> conveyorMachine)
        {
            if (services.Any(service => service.ServiceType == typeof(IDataConveyor<TData, TResult>) || service.ServiceType == typeof(IPriorityDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already registered");
            var conveyorManager = new PriorityDataConveyorManager<TData, TResult>(maxPriority);
            return services.AddSingleton<IDataConveyor<TData, TResult>>(conveyorManager)
                .AddSingleton<IPriorityDataConveyor<TData, TResult>>(conveyorManager)
                .AddHostedService(provider => new SinglePriorityDataConveyorWorker<TData, TResult>(conveyorManager, conveyorMachine));
        }
    }
}