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
    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, IConveyorMachine<TData, TResult> conveyorMachine, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var manager = new ConveyorManager<TData, TResult>(maxAttempts);
        return services.AddSingleton<IConveyor<TData, TResult>>(manager)
                       .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new SingleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachine)));
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services, IConveyorMachine<TData, TResult> conveyorMachine, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority, maxAttempts);
        return services.AddSingleton<IConveyor<TData, TResult>>(manager)
                       .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                       .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new SingleStaticProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachine)));
    }

    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var arguments = conveyorMachines?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(conveyorMachines));
        var manager = new ConveyorManager<TData, TResult>(maxAttempts);
        return arguments.Count switch
        {
            0 => throw new ArgumentOutOfRangeException(nameof(conveyorMachines), conveyorMachines, "Empty collection"),
            1 => services.AddConveyor(arguments.First(), maxAttempts),
            _ => services.AddSingleton<IConveyor<TData, TResult>>(manager)
                         .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(arguments)))
        };
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services, IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var arguments = conveyorMachines?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(conveyorMachines));
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority, maxAttempts);
        return arguments.Count switch
        {
            0 => throw new ArgumentOutOfRangeException(nameof(conveyorMachines), conveyorMachines, "Empty collection"),
            1 => services.AddPriorityConveyor(arguments.First(), maxPriority, maxAttempts),
            _ => services.AddSingleton<IConveyor<TData, TResult>>(manager)
                         .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                         .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, int>(arguments)))
        };
    }

    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFabric, ReuseStrategy strategy, int maxSimultaneous = 1,
        int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (conveyorMachineFabric == null)
            throw new ArgumentNullException(nameof(conveyorMachineFabric));
        var manager = new ConveyorManager<TData, TResult>(maxAttempts);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous <= 1
                ? services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new SingleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric.Invoke(provider))))
                : services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
                              manager,
                              new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, object?>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => conveyorMachineFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxSimultaneous <= 1
                ? services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new SingleReusableProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric)))
                : services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new MultipleReusableProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric, maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous <= 1
                ? services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new SingleOneTimeProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric)))
                : services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider, manager, new MultipleOneTimeProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFabric, maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Incorrect enum value")
        };
    }

    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services, Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFabric, ReuseStrategy strategy,
        int maxSimultaneous = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        if (conveyorMachineFabric == null)
            throw new ArgumentNullException(nameof(conveyorMachineFabric));
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority, maxAttempts);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous <= 1
                ? services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new SingleStaticProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric.Invoke(provider))))
                : services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
                              manager,
                              new MultipleStaticProcessor<IConveyorMachine<TData, TResult>, int>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => conveyorMachineFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxSimultaneous <= 1
                ? services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new SingleReusableProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric)))
                : services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new MultipleReusableProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric, maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous <= 1
                ? services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new SingleOneTimeProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric)))
                : services.AddSingleton<IConveyor<TData, TResult>>(manager)
                          .AddSingleton<IPriorityConveyor<TData, TResult>>(manager)
                          .AddHostedService(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider, manager, new MultipleOneTimeProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFabric, maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Incorrect enum value")
        };
    }

    public static IServiceCollection AddConveyor<TConveyorMachine, TData, TResult>(this IServiceCollection services, ReuseStrategy strategy, int maxSimultaneous = 1, int maxAttempts = int.MaxValue)
        where TConveyorMachine : IConveyorMachine<TData, TResult>
        => services.AddConveyor(provider => provider.GetRequiredService<TConveyorMachine>(), strategy, maxSimultaneous, maxAttempts);

    public static IServiceCollection AddPriorityConveyor<TConveyorMachine, TData, TResult>(this IServiceCollection services, ReuseStrategy strategy, int maxSimultaneous = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TConveyorMachine : IConveyorMachine<TData, TResult>
        => services.AddPriorityConveyor(provider => provider.GetRequiredService<TConveyorMachine>(), strategy, maxSimultaneous, maxPriority, maxAttempts);
}

}
