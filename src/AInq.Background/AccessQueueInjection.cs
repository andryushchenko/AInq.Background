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
using AInq.Background.Processors;
using AInq.Background.Workers;
using System.ComponentModel;

namespace AInq.Background;

/// <summary> Shared resource Access Queue dependency injection </summary>
public static class AccessQueueInjection
{
    /// <summary> Create <see cref="IAccessQueue{TResource}" /> with single static shared resource without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource" /> is NULL </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, TResource resource,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        services.AddHostedService(provider
            => new TaskWorker<TResource, object?>(provider, manager, ProcessorFactory.CreateProcessor<TResource, object?>(resource)));
        return manager;
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}" /> service with single static shared resource </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource" /> is NULL </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateAccessQueue(resource, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityAccessQueue{TResource}" /> with single static shared resource without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource" /> is NULL </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services, TResource resource,
        int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        services.AddHostedService(provider
            => new TaskWorker<TResource, int>(provider, manager, ProcessorFactory.CreateProcessor<TResource, int>(resource)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}" /> and <see cref="IAccessQueue{TResource}" /> services with single static shared resource </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource" /> is NULL </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxPriority = 100,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityAccessQueue(resource, maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(queue).AddSingleton(queue);
    }

    /// <summary> Create <see cref="IAccessQueue{TResource}" /> with static shared resources without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources" /> collection is empty </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        var arguments = (resources ?? throw new ArgumentNullException(nameof(resources))).Where(resource => resource != null).ToList();
        if (arguments.Count == 0)
            throw new ArgumentException("Empty collection", nameof(resources));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        services.AddHostedService(provider
            => new TaskWorker<TResource, object?>(provider, manager, ProcessorFactory.CreateProcessor<TResource, object?>(arguments)));
        return manager;
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}" /> service with static shared resources </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources" /> collection is empty </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateAccessQueue(resources, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityAccessQueue{TResource}" /> with static shared resources without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources" /> collection is empty </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services,
        IEnumerable<TResource> resources, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        var arguments = (resources ?? throw new ArgumentNullException(nameof(resources))).Where(resource => resource != null).ToList();
        if (arguments.Count == 0)
            throw new ArgumentException("Empty collection", nameof(resources));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        services.AddHostedService(provider
            => new TaskWorker<TResource, int>(provider, manager, ProcessorFactory.CreateProcessor<TResource, int>(arguments)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}" /> and <see cref="IAccessQueue{TResource}" /> services with static shared resources </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources" /> collection is empty </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources,
        int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityAccessQueue(resources, maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(queue).AddSingleton(queue);
    }

    /// <summary> Create <see cref="IAccessQueue{TResource}" /> with given shared resources reuse <paramref name="strategy" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFactory"> Shared resource factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFactory" /> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services,
        Func<IServiceProvider, TResource> resourceFactory, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (resourceFactory == null)
            throw new ArgumentNullException(nameof(resourceFactory));
        if (strategy != ReuseStrategy.Static && strategy != ReuseStrategy.Reuse && strategy != ReuseStrategy.OneTime)
            throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
            manager,
            ProcessorFactory.CreateProcessor<TResource, object?>(resourceFactory, strategy, provider, maxResourceInstances)));
        return manager;
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}" /> service with given shared resources reuse <paramref name="strategy" /> </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFactory"> Shared resource factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFactory" /> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFactory,
        ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateAccessQueue(resourceFactory, strategy, maxResourceInstances, maxAttempts));
    }

    /// <summary>
    ///     Create <see cref="IPriorityAccessQueue{TResource}" /> with given shared resources reuse <paramref name="strategy" /> without service
    ///     registration
    /// </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFactory"> Shared resource factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFactory" /> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services,
        Func<IServiceProvider, TResource> resourceFactory, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (resourceFactory == null)
            throw new ArgumentNullException(nameof(resourceFactory));
        if (strategy != ReuseStrategy.Static && strategy != ReuseStrategy.Reuse && strategy != ReuseStrategy.OneTime)
            throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
            manager,
            ProcessorFactory.CreateProcessor<TResource, int>(resourceFactory, strategy, provider, maxResourceInstances)));
        return manager;
    }

    /// <summary>
    ///     Add <see cref="IPriorityAccessQueue{TResource}" /> and <see cref="IAccessQueue{TResource}" /> services with given shared resources reuse
    ///     <paramref name="strategy" />
    /// </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFactory"> Shared resource factory function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFactory" /> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services,
        Func<IServiceProvider, TResource> resourceFactory, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityAccessQueue(resourceFactory, strategy, maxResourceInstances, maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(queue).AddSingleton(queue);
    }

    /// <summary> Create <see cref="IAccessQueue{TResource}" /> with given shared resources reuse <paramref name="strategy" /> without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy,
        int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
        where TResource : notnull
        => services.CreateAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxAttempts);

    /// <summary> Add <see cref="IAccessQueue{TResource}" /> service with given shared resources reuse <paramref name="strategy" /> </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1,
        int maxAttempts = int.MaxValue)
        where TResource : notnull
        => services.AddAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxAttempts);

    /// <summary>
    ///     Create <see cref="IPriorityAccessQueue{TResource}" /> with given shared resources reuse <paramref name="strategy" /> without service
    ///     registration
    /// </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy,
        int maxResourceInstances = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TResource : notnull
        => services.CreatePriorityAccessQueue(provider => provider.GetRequiredService<TResource>(),
            strategy,
            maxResourceInstances,
            maxPriority,
            maxAttempts);

    /// <summary>
    ///     Add <see cref="IPriorityAccessQueue{TResource}" /> and <see cref="IAccessQueue{TResource}" /> services with given shared resources reuse
    ///     <paramref name="strategy" />
    /// </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy" /></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy" /> has incorrect value </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy,
        int maxResourceInstances = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        where TResource : notnull
        => services.AddPriorityAccessQueue(provider => provider.GetRequiredService<TResource>(),
            strategy,
            maxResourceInstances,
            maxPriority,
            maxAttempts);
}
