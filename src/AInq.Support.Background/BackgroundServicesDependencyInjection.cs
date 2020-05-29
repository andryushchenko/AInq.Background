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
using System.Collections.Generic;
using System.Linq;

namespace AInq.Support.Background
{
    public static class BackgroundServicesDependencyInjection
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
                ? services.AddHostedService(provider => new TaskQueueWorker<object, object>(provider, manager, new SingleNullTaskProcessor<object, object>()))
                : services.AddHostedService(provider => new TaskQueueWorker<object, object>(provider, manager, new MultipleNullTaskProcessor<object, object>(maxSimultaneous)));
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
                ? services.AddHostedService(provider => new TaskQueueWorker<object, int>(provider, manager, new SingleNullTaskProcessor<object, int>()))
                : services.AddHostedService(provider => new TaskQueueWorker<object, int>(provider, manager, new MultipleNullTaskProcessor<object, int>(maxSimultaneous)));
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, IDataConveyorMachine<TData, TResult> conveyorMachine)
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            var manager = new DataConveyorManager<TData, TResult>();
            return services.AddSingleton<IDataConveyor<TData, TResult>>(manager)
                .AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(conveyorMachine)));
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IDataConveyorMachine<TData, TResult> conveyorMachine)
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            var manager = new PriorityDataConveyorManager<TData, TResult>(maxPriority);
            return services.AddSingleton<IDataConveyor<TData, TResult>>(manager)
                .AddSingleton<IPriorityDataConveyor<TData, TResult>>(manager)
                .AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(conveyorMachine)));
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, IEnumerable<IDataConveyorMachine<TData, TResult>> conveyorMachines)
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            var arguments = conveyorMachines?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(conveyorMachines));
            switch (arguments.Count)
            {
                case 0:
                    throw new ArgumentException("Empty collection", nameof(conveyorMachines));
                case 1:
                    return services.AddDataConveyor(arguments.First());
                default:
                {
                    var manager = new DataConveyorManager<TData, TResult>();
                    return services.AddSingleton<IDataConveyor<TData, TResult>>(manager)
                        .AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new MultipleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(arguments)));
                }
            }
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IEnumerable<IDataConveyorMachine<TData, TResult>> conveyorMachines)
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            var arguments = conveyorMachines?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(conveyorMachines));
            switch (arguments.Count)
            {
                case 0:
                    throw new ArgumentException("Empty collection", nameof(conveyorMachines));
                case 1:
                    return services.AddPriorityDataConveyor(maxPriority, arguments.First());
                default:
                {
                    var manager = new PriorityDataConveyorManager<TData, TResult>(maxPriority);
                    return services.AddSingleton<IDataConveyor<TData, TResult>>(manager)
                        .AddSingleton<IPriorityDataConveyor<TData, TResult>>(manager)
                        .AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new MultipleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(arguments)));
                }
            }
        }

        public static IServiceCollection AddDataConveyor<TData, TResult>(this IServiceCollection services, Func<IServiceProvider, IDataConveyorMachine<TData, TResult>> conveyorMachineFabric, ReuseStrategy strategy, int maxSimultaneous = 1)
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            if (maxSimultaneous < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
            if (conveyorMachineFabric == null)
                throw new ArgumentNullException(nameof(conveyorMachineFabric));
            var manager = new DataConveyorManager<TData, TResult>();
            services.AddSingleton<IDataConveyor<TData, TResult>>(manager);
            return strategy switch
            {
                ReuseStrategy.Static => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(conveyorMachineFabric.Invoke(provider))))
                    : services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new MultipleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => conveyorMachineFabric.Invoke(provider))))),
                ReuseStrategy.Reuse => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleReusableTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(conveyorMachineFabric)))
                    : throw new NotImplementedException(),
                ReuseStrategy.OneTime => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleOneTimeTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(conveyorMachineFabric)))
                    : throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
            };
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, Func<IServiceProvider, IDataConveyorMachine<TData, TResult>> conveyorMachineFabric, ReuseStrategy strategy, int maxSimultaneous = 1)
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            if (maxSimultaneous < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
            if (conveyorMachineFabric == null)
                throw new ArgumentNullException(nameof(conveyorMachineFabric));
            var manager = new PriorityDataConveyorManager<TData, TResult>(maxPriority);
            services.AddSingleton<IDataConveyor<TData, TResult>>(manager)
                .AddSingleton<IPriorityDataConveyor<TData, TResult>>(manager);
            return strategy switch
            {
                ReuseStrategy.Static => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(conveyorMachineFabric.Invoke(provider))))
                    : services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new MultipleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => conveyorMachineFabric.Invoke(provider)).Where(machine => machine != null)))),
                ReuseStrategy.Reuse => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleReusableTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(conveyorMachineFabric)))
                    : throw new NotImplementedException(),
                ReuseStrategy.OneTime => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleOneTimeTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(conveyorMachineFabric)))
                    : throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
            };
        }

        public static IServiceCollection AddDataConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, ReuseStrategy strategy, int maxSimultaneous = 1) where TConveyorMachine:IDataConveyorMachine<TData, TResult>
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            if (maxSimultaneous < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
            var manager = new DataConveyorManager<TData, TResult>();
            services.AddSingleton<IDataConveyor<TData, TResult>>(manager);
            return strategy switch
            {
                ReuseStrategy.Static => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(provider.GetService<TConveyorMachine>())))
                    : services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new MultipleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => provider.GetService<TConveyorMachine>() as IDataConveyorMachine<TData, TResult>).Where(machine => machine != null)))),
                ReuseStrategy.Reuse => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleReusableTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(serviceProvider => serviceProvider.GetService<TConveyorMachine>())))
                    : throw new NotImplementedException(),
                ReuseStrategy.OneTime => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, object>(provider, manager, new SingleOneTimeTaskProcessor<IDataConveyorMachine<TData, TResult>, object>(serviceProvider => serviceProvider.GetService<TConveyorMachine>())))
                    : throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
            };
        }

        public static IServiceCollection AddPriorityDataConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, int maxPriority, ReuseStrategy strategy, int maxSimultaneous = 1) where TConveyorMachine:IDataConveyorMachine<TData, TResult>
        {
            if (services.Any(service => service.ImplementationType == typeof(IDataConveyor<TData, TResult>)))
                throw new InvalidOperationException("Service already exists");
            if (maxPriority < 0)
                throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
            if (maxSimultaneous < 1)
                throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
            var manager = new PriorityDataConveyorManager<TData, TResult>(maxPriority);
            services.AddSingleton<IDataConveyor<TData, TResult>>(manager)
                .AddSingleton<IPriorityDataConveyor<TData, TResult>>(manager);
            return strategy switch
            {
                ReuseStrategy.Static => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(provider.GetService<TConveyorMachine>())))
                    : services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new MultipleStaticTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => provider.GetService<TConveyorMachine>() as IDataConveyorMachine<TData, TResult>).Where(machine => machine != null)))),
                ReuseStrategy.Reuse => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleReusableTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(serviceProvider => serviceProvider.GetService<TConveyorMachine>())))
                    : throw new NotImplementedException(),
                ReuseStrategy.OneTime => maxSimultaneous == 1
                    ? services.AddHostedService(provider => new TaskQueueWorker<IDataConveyorMachine<TData, TResult>, int>(provider, manager, new SingleReusableTaskProcessor<IDataConveyorMachine<TData, TResult>, int>(serviceProvider => serviceProvider.GetService<TConveyorMachine>())))
                    : throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
            };
        }
    }
}