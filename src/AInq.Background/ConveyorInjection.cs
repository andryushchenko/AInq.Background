// Copyright 2020 Anton Andryushchenko
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
using AInq.Background.Workers;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using static AInq.Background.Processors.ProcessorFactory;

namespace AInq.Background;

/// <summary> Background data processing Conveyor dependency injection </summary>
public static class ConveyorInjection
{
    /// <summary> Create <see cref="IConveyor{TData, TResult}" /> with single static conveyor machine without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachine"> Conveyor machine instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    [PublicAPI]
    public static IConveyor<TData, TResult> CreateConveyor<TData, TResult>(this IServiceCollection services,
        IConveyorMachine<TData, TResult> conveyorMachine, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = conveyorMachine ?? throw new ArgumentNullException(nameof(conveyorMachine));
        var manager = new ConveyorManager<TData, TResult>(maxAttempts);
        services.AddSingleton<IHostedService>(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
            manager,
            CreateProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachine)));
        return manager;
    }

    /// <summary> Add <see cref="IConveyor{TData, TResult}" /> service with single static conveyor machine </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachine"> Conveyor machine instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    [PublicAPI]
    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services, IConveyorMachine<TData, TResult> conveyorMachine,
        int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateConveyor(conveyorMachine, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityConveyor{TData, TResult}" /> with single static conveyor machine without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachine"> Conveyor machine instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    [PublicAPI]
    public static IPriorityConveyor<TData, TResult> CreatePriorityConveyor<TData, TResult>(this IServiceCollection services,
        IConveyorMachine<TData, TResult> conveyorMachine, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = conveyorMachine ?? throw new ArgumentNullException(nameof(conveyorMachine));
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority, maxAttempts);
        services.AddSingleton<IHostedService>(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
            manager,
            CreateProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachine)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityConveyor{TData, TResult}" /> and <see cref="IConveyor{TData, TResult}" /> services with single static conveyor machine </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachine"> Conveyor machine instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    [PublicAPI]
    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services,
        IConveyorMachine<TData, TResult> conveyorMachine, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var conveyor = services.CreatePriorityConveyor(conveyorMachine, maxPriority, maxAttempts);
        return services.AddSingleton<IConveyor<TData, TResult>>(conveyor).AddSingleton(conveyor);
    }

    /// <summary> Create <see cref="IConveyor{TData, TResult}" /> with static conveyor machines without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachines"> Conveyor machines collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="conveyorMachines" /> collection is empty </exception>
    [PublicAPI]
    public static IConveyor<TData, TResult> CreateConveyor<TData, TResult>(this IServiceCollection services,
        IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        var arguments = (conveyorMachines ?? throw new ArgumentNullException(nameof(conveyorMachines)))
                        .Select(machine => machine ?? throw new ArgumentNullException(nameof(machine)))
                        .ToList();
        if (arguments.Count == 0) throw new ArgumentException("Empty collection", nameof(conveyorMachines));
        var manager = new ConveyorManager<TData, TResult>(maxAttempts);
        services.AddSingleton<IHostedService>(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
            manager,
            CreateProcessor<IConveyorMachine<TData, TResult>, object?>(arguments)));
        return manager;
    }

    /// <summary> Add <see cref="IConveyor{TData, TResult}" /> service with static conveyor machines </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachines"> Conveyor machines collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="conveyorMachines" /> collection is empty </exception>
    [PublicAPI]
    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services,
        IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateConveyor(conveyorMachines, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityConveyor{TData, TResult}" /> with static conveyor machines without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachines"> Conveyor machines collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="conveyorMachines" /> collection is empty </exception>
    [PublicAPI]
    public static IPriorityConveyor<TData, TResult> CreatePriorityConveyor<TData, TResult>(this IServiceCollection services,
        IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        var arguments = (conveyorMachines ?? throw new ArgumentNullException(nameof(conveyorMachines)))
                        .Select(machine => machine ?? throw new ArgumentNullException(nameof(machine)))
                        .ToList();
        if (arguments.Count == 0) throw new ArgumentException("Empty collection", nameof(conveyorMachines));
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority, maxAttempts);
        services.AddSingleton<IHostedService>(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
            manager,
            CreateProcessor<IConveyorMachine<TData, TResult>, int>(arguments)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityConveyor{TData, TResult}" /> and <see cref="IConveyor{TData, TResult}" /> services with static conveyor machines </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachines"> Conveyor machines collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="conveyorMachines" /> collection is empty </exception>
    [PublicAPI]
    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services,
        IEnumerable<IConveyorMachine<TData, TResult>> conveyorMachines, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var conveyor = services.CreatePriorityConveyor(conveyorMachines, maxPriority, maxAttempts);
        return services.AddSingleton<IConveyor<TData, TResult>>(conveyor).AddSingleton(conveyor);
    }

    /// <summary> Create <see cref="IConveyor{TData, TResult}" /> with given conveyor machines reuse <paramref name="strategy" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachineFactory"> Conveyor machine factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IConveyor<TData, TResult> CreateConveyor<TData, TResult>(this IServiceCollection services,
        Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFactory, ReuseStrategy strategy, int maxParallelMachines = 1,
        int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = conveyorMachineFactory ?? throw new ArgumentNullException(nameof(conveyorMachineFactory));
        if (strategy != ReuseStrategy.Static && strategy != ReuseStrategy.Reuse && strategy != ReuseStrategy.OneTime)
            throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy));
        var manager = new ConveyorManager<TData, TResult>(maxAttempts);
        services.AddSingleton<IHostedService>(provider => new TaskWorker<IConveyorMachine<TData, TResult>, object?>(provider,
            manager,
            CreateProcessor<IConveyorMachine<TData, TResult>, object?>(conveyorMachineFactory, strategy, provider, maxParallelMachines)));
        return manager;
    }

    /// <summary> Add <see cref="IConveyor{TData, TResult}" /> service with given conveyor machines reuse <paramref name="strategy" /> </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachineFactory"> Conveyor machine factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IServiceCollection AddConveyor<TData, TResult>(this IServiceCollection services,
        Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFactory, ReuseStrategy strategy, int maxParallelMachines = 1,
        int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateConveyor(conveyorMachineFactory, strategy, maxParallelMachines, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityConveyor{TData, TResult}" /> with given conveyor machines reuse <paramref name="strategy" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachineFactory"> Conveyor machine factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IPriorityConveyor<TData, TResult> CreatePriorityConveyor<TData, TResult>(this IServiceCollection services,
        Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFactory, ReuseStrategy strategy, int maxParallelMachines = 1,
        int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = conveyorMachineFactory ?? throw new ArgumentNullException(nameof(conveyorMachineFactory));
        if (strategy != ReuseStrategy.Static && strategy != ReuseStrategy.Reuse && strategy != ReuseStrategy.OneTime)
            throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy));
        var manager = new PriorityConveyorManager<TData, TResult>(maxPriority, maxAttempts);
        services.AddSingleton<IHostedService>(provider => new TaskWorker<IConveyorMachine<TData, TResult>, int>(provider,
            manager,
            CreateProcessor<IConveyorMachine<TData, TResult>, int>(conveyorMachineFactory, strategy, provider, maxParallelMachines)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityConveyor{TData, TResult}" /> and <see cref="IConveyor{TData, TResult}" /> services with given conveyor machines reuse <paramref name="strategy" /> </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="conveyorMachineFactory"> Conveyor machine factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IServiceCollection AddPriorityConveyor<TData, TResult>(this IServiceCollection services,
        Func<IServiceProvider, IConveyorMachine<TData, TResult>> conveyorMachineFactory, ReuseStrategy strategy, int maxParallelMachines = 1,
        int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        if (services.Any(service => service.ImplementationType == typeof(IConveyor<TData, TResult>)))
            throw new InvalidOperationException("Service already exists");
        var conveyor = services.CreatePriorityConveyor(conveyorMachineFactory, strategy, maxParallelMachines, maxPriority, maxAttempts);
        return services.AddSingleton<IConveyor<TData, TResult>>(conveyor).AddSingleton(conveyor);
    }

    /// <summary> Create <see cref="IConveyor{TData, TResult}" /> with given conveyor machines reuse <paramref name="strategy" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <typeparam name="TConveyorMachine"> Conveyor machine type </typeparam>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IConveyor<TData, TResult> CreateConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, ReuseStrategy strategy,
        int maxParallelMachines = 1, int maxAttempts = int.MaxValue)
        where TData : notnull
        where TConveyorMachine : IConveyorMachine<TData, TResult>
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .CreateConveyor(provider => provider.GetRequiredService<TConveyorMachine>(), strategy, maxParallelMachines, maxAttempts);

    /// <summary> Add <see cref="IConveyor{TData, TResult}" /> service with given conveyor machines reuse <paramref name="strategy" /> </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <typeparam name="TConveyorMachine"> Conveyor machine type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IServiceCollection AddConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, ReuseStrategy strategy,
        int maxParallelMachines = 1, int maxAttempts = int.MaxValue)
        where TData : notnull
        where TConveyorMachine : IConveyorMachine<TData, TResult>
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddConveyor(provider => provider.GetRequiredService<TConveyorMachine>(), strategy, maxParallelMachines, maxAttempts);

    /// <summary> Create <see cref="IPriorityConveyor{TData, TResult}" /> with given conveyor machines reuse <paramref name="strategy" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <typeparam name="TConveyorMachine"> Conveyor machine type </typeparam>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IPriorityConveyor<TData, TResult> CreatePriorityConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services,
        ReuseStrategy strategy, int maxParallelMachines = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
        where TConveyorMachine : IConveyorMachine<TData, TResult>
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .CreatePriorityConveyor(provider => provider.GetRequiredService<TConveyorMachine>(),
                strategy,
                maxParallelMachines,
                maxPriority,
                maxAttempts);

    /// <summary> Add <see cref="IPriorityConveyor{TData, TResult}" /> and <see cref="IConveyor{TData, TResult}" /> services with given conveyor machines reuse <paramref name="strategy" /> </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /> </param>
    /// <param name="maxParallelMachines"> Max allowed parallel conveyor machines </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <typeparam name="TConveyorMachine"> Conveyor machine type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    [PublicAPI]
    public static IServiceCollection AddPriorityConveyor<TData, TResult, TConveyorMachine>(this IServiceCollection services, ReuseStrategy strategy,
        int maxParallelMachines = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TData : notnull
        where TConveyorMachine : IConveyorMachine<TData, TResult>
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddPriorityConveyor(provider => provider.GetRequiredService<TConveyorMachine>(),
                strategy,
                maxParallelMachines,
                maxPriority,
                maxAttempts);
}
