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
using System.Collections.Generic;
using System.Linq;

namespace AInq.Background
{

public static class ConveyorInjection
{
    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, IConveyorMachine<TData, TResult> conveyorMachine)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var manager = new ConveyorManager<TData, TResult>();
        return services.AddSingleton<IConveyor<TData, TResult>>(manager)
                       .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                           manager,
                           new SingleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachine)));
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IConveyorMachine<TData, TResult> conveyorMachine)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority);
        return services.AddSingleton<IConveyor<TData, TResult>>(manager)
                       .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                       .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                           manager,
                           new SingleStaticProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachine)));
    }

    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var arguments = conveyorMachines?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(conveyorMachines));
        switch (arguments.Count)
        {
            case 0:
                throw new ArgumentException("Empty collection", nameof(conveyorMachines));
            case 1:
                return services.AddConveyor(arguments.First());
            default:
                var manager = new ConveyorManager<TData, TResult>();
                return services.AddSingleton<IConveyor<TData, TResult>>(manager)
                               .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                                   manager,
                                   new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(arguments)));
        }
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services, int maxPriority, IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        var arguments = conveyorMachines?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(conveyorMachines));
        switch (arguments.Count)
        {
            case 0:
                throw new ArgumentException("Empty collection", nameof(conveyorMachines));
            case 1:
                return services.AddPriorityConveyor(maxPriority, arguments.First());
            default:
                var manager = new PriorityConveyorManager<TData, TResult>(maxPriority);
                return services.AddSingleton<IConveyor<TData, TResult>>(manager)
                               .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                               .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                                   manager,
                                   new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, int>(arguments)));
        }
    }

    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFabric,
        ReuseStrategy strategy, int maxSimultaneous = 1)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        if (conveyorMachineFabric == null)
            throw new ArgumentNullException(nameof(conveyorMachineFabric));
        var manager = new ConveyorManager<TData, TResult>();
        services.AddSingleton<IConveyor<TData, TResult>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new SingleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric.Invoke(provider))))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => conveyorMachineFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new SingleReusableProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric)))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new MultipleReusableProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric, maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new SingleOneTimeProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric)))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new MultipleOneTimeProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric, maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services, int maxPriority,
        Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFabric, ReuseStrategy strategy,
        int maxSimultaneous = 1)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        if (conveyorMachineFabric == null)
            throw new ArgumentNullException(nameof(conveyorMachineFabric));
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority);
        services.AddSingleton<IConveyor<TData, TResult>>(manager)
                .AddSingleton<IPriorityConveyor<TData, TResult>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new SingleStaticProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric.Invoke(provider))))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, int>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => conveyorMachineFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new SingleReusableProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric)))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new MultipleReusableProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric, maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new SingleOneTimeProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric)))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new MultipleOneTimeProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric, maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    public static IServiceCollection AddConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, ReuseStrategy strategy, int maxSimultaneous = 1)
        where TConveyorMachine : IConveyorMachine<TData, TResult>
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        var manager = new ConveyorManager<TData, TResult>();
        services.AddSingleton<IConveyor<TData, TResult>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new SingleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(provider.GetRequiredService<TConveyorMachine>())))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(Enumerable
                                                                                           .Repeat(0, maxSimultaneous)
                                                                                           .Select(_ => provider.GetRequiredService<TConveyorMachine>() as IConveyorMachine<TData, TResult>)))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new SingleReusableProcessor<IConveyorMachine<TData, TResult>, object?>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>())))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new MultipleReusableProcessor<IConveyorMachine<TData, TResult>, object?>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>(), maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new SingleOneTimeProcessor<IConveyorMachine<TData, TResult>, object?>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>())))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                    manager,
                    new MultipleOneTimeProcessor<IConveyorMachine<TData, TResult>, object?>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>(), maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, int maxPriority, ReuseStrategy strategy, int maxSimultaneous = 1)
        where TConveyorMachine : IConveyorMachine<TData, TResult>
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority);
        services.AddSingleton<IConveyor<TData, TResult>>(manager)
                .AddSingleton<IPriorityConveyor<TData, TResult>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new SingleStaticProcessor<IConveyorMachine<TData, TResult>, int>(provider.GetRequiredService<TConveyorMachine>())))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, int>(Enumerable
                                                                                       .Repeat(0, maxSimultaneous)
                                                                                       .Select(_ => provider.GetRequiredService<TConveyorMachine>() as IConveyorMachine<TData, TResult>)))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new SingleReusableProcessor<IConveyorMachine<TData, TResult>, int>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>())))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new MultipleReusableProcessor<IConveyorMachine<TData, TResult>, int>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>(), maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new SingleOneTimeProcessor<IConveyorMachine<TData, TResult>, int>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>())))
                : services.AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                    manager,
                    new MultipleOneTimeProcessor<IConveyorMachine<TData, TResult>, int>(serviceProvider => serviceProvider.GetRequiredService<TConveyorMachine>(), maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }
}

}
