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
using AInq.Background.Processors;
using AInq.Background.Workers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AInq.Background
{

/// <summary> Shared resource Access Queue dependency injection </summary>
public static class AccessQueueInjection
{
    /// <summary> Create <see cref="IAccessQueue{TResource}"/> with single static shared resource without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource"/> is NULL </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxAttempts = int.MaxValue)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(resource)));
        return manager;
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with single static shared resource </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource"/> is NULL </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateAccessQueue(resource, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityAccessQueue{TResource}"/> with single static shared resource without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource"/> is NULL </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(resource)));
        return manager;
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with single static shared resource </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource"/> is NULL </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityAccessQueue(resource, maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(queue).AddSingleton(queue);
    }

    /// <summary> Create <see cref="IAccessQueue{TResource}"/> with static shared resources without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources"/> collection is empty </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources, int maxAttempts = int.MaxValue)
    {
        var arguments = resources?.Where(resource => resource != null).ToList() ?? throw new ArgumentNullException(nameof(resources));

        switch (arguments.Count)
        {
            case 0:
                throw new ArgumentException("Empty collection", nameof(resources));
            case 1:
                return services.CreateAccessQueue(arguments.First());
            default:
                var manager = new AccessQueueManager<TResource>(maxAttempts);
                services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleStaticProcessor<TResource, object?>(arguments)));
                return manager;
        }
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with static shared resources </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources"/> collection is empty </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateAccessQueue(resources, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityAccessQueue{TResource}"/> with static shared resources without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources"/> collection is empty </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        var arguments = resources?.Where(resource => resource != null).ToList() ?? throw new ArgumentNullException(nameof(resources));
        switch (arguments.Count)
        {
            case 0:
                throw new ArgumentException("Empty collection", nameof(resources));
            case 1:
                return services.CreatePriorityAccessQueue(arguments.First(), maxPriority);
            default:
                var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
                services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleStaticProcessor<TResource, int>(arguments)));
                return manager;
        }
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with static shared resources </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources"/> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="resources"/> collection is empty </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityAccessQueue(resources, maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(queue).AddSingleton(queue);
    }

    /// <summary> Create <see cref="IAccessQueue{TResource}"/> with given shared resources reuse strategy without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFabric"> Shared resource fabric function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFabric"/> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
    {
        if (resourceFabric == null)
            throw new ArgumentNullException(nameof(resourceFabric));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        switch (strategy)
        {
            case ReuseStrategy.Static:
                if (maxResourceInstances <= 1)
                    services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(resourceFabric.Invoke(provider))));
                else
                    services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                        manager,
                        new MultipleStaticProcessor<TResource, object?>(Enumerable.Repeat(0, maxResourceInstances).Select(_ => resourceFabric.Invoke(provider)))));
                break;
            case ReuseStrategy.Reuse:
                if (maxResourceInstances <= 1)
                    services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleReusableProcessor<TResource, object?>(resourceFabric)));
                else services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleReusableProcessor<TResource, object?>(resourceFabric, maxResourceInstances)));
                break;
            case ReuseStrategy.OneTime:
                if (maxResourceInstances <= 1)
                    services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleOneTimeProcessor<TResource, object?>(resourceFabric)));
                else services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleOneTimeProcessor<TResource, object?>(resourceFabric, maxResourceInstances)));
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy));
        }
        return manager;
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFabric"> Shared resource fabric function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFabric"/> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        return services.AddSingleton(services.CreateAccessQueue(resourceFabric, strategy, maxResourceInstances, maxAttempts));
    }

    /// <summary> Create <see cref="IPriorityAccessQueue{TResource}"/> with given shared resources reuse strategy without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFabric"> Shared resource fabric function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFabric"/> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100,
        int maxAttempts = int.MaxValue)
    {
        if (resourceFabric == null)
            throw new ArgumentNullException(nameof(resourceFabric));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        switch (strategy)
        {
            case ReuseStrategy.Static:
                if (maxResourceInstances <= 1)
                    services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(resourceFabric.Invoke(provider))));
                else services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleStaticProcessor<TResource, int>(Enumerable.Repeat(0, maxResourceInstances).Select(_ => resourceFabric.Invoke(provider)))));
                break;
            case ReuseStrategy.Reuse:
                if (maxResourceInstances <= 1)
                    services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleReusableProcessor<TResource, int>(resourceFabric)));
                else services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleReusableProcessor<TResource, int>(resourceFabric, maxResourceInstances)));
                break;
            case ReuseStrategy.OneTime:
                if (maxResourceInstances <= 1)
                    services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleOneTimeProcessor<TResource, int>(resourceFabric)));
                else services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleOneTimeProcessor<TResource, int>(resourceFabric, maxResourceInstances)));
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy));
        }
        return manager;
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFabric"> Shared resource fabric function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFabric"/> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100,
        int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var queue = services.CreatePriorityAccessQueue(resourceFabric, strategy, maxResourceInstances, maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(queue).AddSingleton(queue);
    }

    /// <summary> Create <see cref="IAccessQueue{TResource}"/> with given shared resources reuse strategy without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IAccessQueue<TResource> CreateAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
        => services.CreateAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxAttempts);

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
        => services.AddAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxAttempts);

    /// <summary> Create <see cref="IPriorityAccessQueue{TResource}"/> with given shared resources reuse strategy without service registration </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IPriorityAccessQueue<TResource> CreatePriorityAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        => services.CreatePriorityAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxPriority, maxAttempts);

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        => services.AddPriorityAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxPriority, maxAttempts);
}

}
