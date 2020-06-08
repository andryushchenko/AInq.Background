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

public static class AccessQueueInjection
{
    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, TResource resource)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var manager = new AccessQueueManager<TResource>();
        return services.AddSingleton<IAccessQueue<TResource>>(manager)
                       .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(resource)));
    }

    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, int maxPriority, TResource resource)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority);
        return services.AddSingleton<IAccessQueue<TResource>>(manager)
                       .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                       .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(resource)));
    }

    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, IEnumerable<TResource> resources)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        var arguments = resources?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(resources));
        switch (arguments.Count)
        {
            case 0:
                throw new ArgumentException("Empty collection", nameof(resources));
            case 1:
                return services.AddAccessQueue(arguments.First());
            default:
            {
                var manager = new AccessQueueManager<TResource>();
                return services.AddSingleton<IAccessQueue<TResource>>(manager)
                               .AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleStaticProcessor<TResource, object?>(arguments)));
            }
        }
    }

    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, int maxPriority, IEnumerable<TResource> resources)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        var arguments = resources?.Where(machine => machine != null).ToList() ?? throw new ArgumentNullException(nameof(resources));
        switch (arguments.Count)
        {
            case 0:
                throw new ArgumentException("Empty collection", nameof(resources));
            case 1:
                return services.AddPriorityAccessQueue(maxPriority, arguments.First());
            default:
            {
                var manager = new PriorityAccessQueueManager<TResource>(maxPriority);
                return services.AddSingleton<IAccessQueue<TResource>>(manager)
                               .AddSingleton<IPriorityAccessQueue<TResource>>(manager)
                               .AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleStaticProcessor<TResource, int>(arguments)));
            }
        }
    }

    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy, int maxSimultaneous = 1)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        if (resourceFabric == null)
            throw new ArgumentNullException(nameof(resourceFabric));
        var manager = new AccessQueueManager<TResource>();
        services.AddSingleton<IAccessQueue<TResource>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(resourceFabric.Invoke(provider))))
                : services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                    manager,
                    new MultipleStaticProcessor<TResource, object?>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => resourceFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleReusableProcessor<TResource, object?>(resourceFabric)))
                : services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleReusableProcessor<TResource, object?>(resourceFabric, maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleOneTimeProcessor<TResource, object?>(resourceFabric)))
                : services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new MultipleOneTimeProcessor<TResource, object?>(resourceFabric, maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, int maxPriority, Func<IServiceProvider, TResource> resourceFabric, ReuseStrategy strategy,
        int maxSimultaneous = 1)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        if (resourceFabric == null)
            throw new ArgumentNullException(nameof(resourceFabric));
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority);
        services.AddSingleton<IAccessQueue<TResource>>(manager)
                .AddSingleton<IPriorityAccessQueue<TResource>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(resourceFabric.Invoke(provider))))
                : services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                    manager,
                    new MultipleStaticProcessor<TResource, int>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => resourceFabric.Invoke(provider))))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleReusableProcessor<TResource, int>(resourceFabric)))
                : services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleReusableProcessor<TResource, int>(resourceFabric, maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleOneTimeProcessor<TResource, int>(resourceFabric)))
                : services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new MultipleOneTimeProcessor<TResource, int>(resourceFabric, maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    public static IServiceCollection AddAccessQueue<TResource>(this IServiceCollection services, ReuseStrategy strategy, int maxSimultaneous = 1)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        var manager = new AccessQueueManager<TResource>();
        services.AddSingleton<IAccessQueue<TResource>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider, manager, new SingleStaticProcessor<TResource, object?>(provider.GetRequiredService<TResource>())))
                : services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                    manager,
                    new MultipleStaticProcessor<TResource, object?>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => provider.GetRequiredService<TResource>())))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                    manager,
                    new SingleReusableProcessor<TResource, object?>(serviceProvider => serviceProvider.GetRequiredService<TResource>())))
                : services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                    manager,
                    new MultipleReusableProcessor<TResource, object?>(serviceProvider => serviceProvider.GetRequiredService<TResource>(),
                        maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                    manager,
                    new SingleOneTimeProcessor<TResource, object?>(serviceProvider => serviceProvider.GetRequiredService<TResource>())))
                : services.AddHostedService(provider => new TaskWorker<TResource, object?>(provider,
                    manager,
                    new MultipleOneTimeProcessor<TResource, object?>(serviceProvider => serviceProvider.GetRequiredService<TResource>(),
                        maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }

    public static IServiceCollection AddPriorityAccessQueue<TResource>(this IServiceCollection services, int maxPriority, ReuseStrategy strategy, int maxSimultaneous = 1)
    {
        if (services.Any(service => service.ImplementationType == typeof(IAccessQueue<TResource>)))
            throw new InvalidOperationException("Service already exists");
        if (maxPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPriority), maxPriority, null);
        if (maxSimultaneous < 1)
            throw new ArgumentOutOfRangeException(nameof(maxSimultaneous), maxSimultaneous, null);
        var manager = new PriorityAccessQueueManager<TResource>(maxPriority);
        services.AddSingleton<IAccessQueue<TResource>>(manager)
                .AddSingleton<IPriorityAccessQueue<TResource>>(manager);
        return strategy switch
        {
            ReuseStrategy.Static => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, int>(provider, manager, new SingleStaticProcessor<TResource, int>(provider.GetRequiredService<TResource>())))
                : services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                    manager,
                    new MultipleStaticProcessor<TResource, int>(Enumerable.Repeat(0, maxSimultaneous).Select(_ => provider.GetRequiredService<TResource>())))),
            ReuseStrategy.Reuse => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                    manager,
                    new SingleReusableProcessor<TResource, int>(serviceProvider => serviceProvider.GetRequiredService<TResource>())))
                : services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                    manager,
                    new MultipleReusableProcessor<TResource, int>(serviceProvider => serviceProvider.GetRequiredService<TResource>(),
                        maxSimultaneous))),
            ReuseStrategy.OneTime => maxSimultaneous == 1
                ? services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                    manager,
                    new SingleOneTimeProcessor<TResource, int>(serviceProvider => serviceProvider.GetRequiredService<TResource>())))
                : services.AddHostedService(provider => new TaskWorker<TResource, int>(provider,
                    manager,
                    new MultipleOneTimeProcessor<TResource, int>(serviceProvider => serviceProvider.GetRequiredService<TResource>(),
                        maxSimultaneous))),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }
}

}
