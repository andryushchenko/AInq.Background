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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background
{

public static class AccessQueueHelper
{
    public static Task EnqueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAccess(access, cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task EnqueueAccess<TResource, TAccess>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAccess : IAccess<TResource>
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAccess<TAccess>(priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAccess<TAccess>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task<TResult> EnqueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAccess(access, cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAccess : IAccess<TResource, TResult>
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAccess<TAccess, TResult>(priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAccess<TAccess, TResult>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task EnqueueAsyncAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncAccess : IAsyncAccess<TResource>
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAsyncAccess<TAsyncAccess>(priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAsyncAccess<TAsyncAccess>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task<TResult> EnqueueAsyncAccess<TResource, TResult>(this IServiceProvider provider, IAsyncAccess<TResource, TResult> access, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }

    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
    {
        var service = provider.GetService(typeof(IPriorityAccessQueue<TResource>)) ?? provider.GetService(typeof(IAccessQueue<TResource>));
        return service switch
        {
            IPriorityAccessQueue<TResource> priorityAccessQueue => priorityAccessQueue.EnqueueAsyncAccess<TAsyncAccess, TResult>(priority, cancellation, attemptsCount),
            IAccessQueue<TResource> accessQueue => accessQueue.EnqueueAsyncAccess<TAsyncAccess, TResult>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found")
        };
    }
}

}
