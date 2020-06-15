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

/// <summary>
/// Shared resource Access Queue dependency injection
/// </summary>
public static class AccessQueueInjection
{
    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with single static shared resource </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource"/> is NULL </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(manager)
                       .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(resource)));
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with single static shared resource </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resource"/> is NULL </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, TResource resource, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        return services.AddSingleton<IAccessQueue<TResource>>(manager)
                       .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                       .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(resource)));
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with static shared resources </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="resources"/> collection is empty </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var arguments = resources?.Where(resource => resource != null).ToList() ?? throw new ArgumentNullException(nameof(resources));
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        return arguments.Count switch
        {
            0 => throw new ArgumentOutOfRangeException(nameof(resources), resources, "Empty collection"),
            1 => services.AddAccessQueue(arguments.First()),
            _ => services.AddSingleton<IAccessQueue<TResource>>(manager)
                         .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleStaticProcessor<TResource, object?>(arguments)))
        };
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with static shared resources </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resources"> Shared resources collection </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resources"/> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="resources"/> collection is empty </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources, int maxPriority = 100, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var arguments = resources?.Where(resource => resource != null).ToList() ?? throw new ArgumentNullException(nameof(resources));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        return arguments.Count switch
        {
            0 => throw new ArgumentOutOfRangeException(nameof(resources), resources, "Empty collection"),
            1 => services.AddPriorityAccessQueue(arguments.First(), maxPriority),
            _ => services.AddSingleton<IAccessQueue<TResource>>(manager)
                         .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                         .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleStaticProcessor<TResource, int>(arguments)))
        };
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFabric"> Shared resource fabric function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFabric"/> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var manager = new AccessQueueManager<TResource>(maxAttempts);
        return strategy switch
        {
            ReuseStrategy.Static => maxResourceInstances <= 1
                ? services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(resourceFabric.Invoke(provider))))
                : services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                              manager,
                              new MultipleStaticProcessor<TResource, object?>(Enumerable.Repeat(0, maxResourceInstances).Select(_ => resourceFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxResourceInstances <= 1
                ? services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleReusableProcessor<TResource, object?>(resourceFabric)))
                : services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleReusableProcessor<TResource, object?>(resourceFabric, maxResourceInstances))),
            ReuseStrategy.OneTime => maxResourceInstances <= 1
                ? services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleOneTimeProcessor<TResource, object?>(resourceFabric)))
                : services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleOneTimeProcessor<TResource, object?>(resourceFabric, maxResourceInstances))),
            _ => throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy))
        };
    }

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="resourceFabric"> Shared resource fabric function </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="resourceFabric"/> is NULL </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100,
        int maxAttempts = int.MaxValue)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (resourceFabric == null)
            throw new ArgumentNullException(nameof(resourceFabric));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority, maxAttempts);
        return strategy switch
        {
            ReuseStrategy.Static => maxResourceInstances <= 1
                ? services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(resourceFabric.Invoke(provider))))
                : services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                              manager,
                              new MultipleStaticProcessor<TResource, int>(Enumerable.Repeat(0, maxResourceInstances).Select(_ => resourceFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxResourceInstances <= 1
                ? services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleReusableProcessor<TResource, int>(resourceFabric)))
                : services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleReusableProcessor<TResource, int>(resourceFabric, maxResourceInstances))),
            ReuseStrategy.OneTime => maxResourceInstances <= 1
                ? services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleOneTimeProcessor<TResource, int>(resourceFabric)))
                : services.AddSingleton<IAccessQueue<TResource>>(manager)
                          .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                          .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleOneTimeProcessor<TResource, int>(resourceFabric, maxResourceInstances))),
            _ => throw new InvalidEnumArgumentException(nameof(strategy), (int) strategy, typeof(ReuseStrategy))
        };
    }

    /// <summary> Add <see cref="IAccessQueue{TResource}"/> service with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1, int maxAttempts = int.MaxValue)
        => services.AddAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxAttempts);

    /// <summary> Add <see cref="IPriorityAccessQueue{TResource}"/> and <see cref="IAccessQueue{TResource}"/> services with given shared resources reuse strategy </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="strategy"> Conveyor machines reuse strategy <seealso cref="ReuseStrategy"/></param>
    /// <param name="maxResourceInstances"> Max allowed shared resource instances count </param>
    /// <param name="maxPriority"> Max allowed operation priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    /// <typeparam name="TResource"></typeparam>
    /// <exception cref="InvalidOperationException"> Thrown if service already exists </exception>
    /// <exception cref="InvalidEnumArgumentException"> Thrown if <paramref name="strategy"/> has incorrect value </exception>
    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxResourceInstances = 1, int maxPriority = 100, int maxAttempts = int.MaxValue)
        => services.AddPriorityAccessQueue(provider => provider.GetRequiredService<TResource>(), strategy, maxResourceInstances, maxPriority, maxAttempts);
}

}
