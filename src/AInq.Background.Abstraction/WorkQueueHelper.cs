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

public static class WorkQueueHelper
{
    public static Task EnqueueWork(this IServiceProvider provider, IWork work, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task EnqueueWork<TWork>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork<TWork>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork<TWork>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task<TResult> EnqueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task<TResult> EnqueueWork<TWork, TResult>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork<TWork, TResult>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork<TWork, TResult>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task EnqueueAsyncWork(this IServiceProvider provider, IAsyncWork work, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task EnqueueAsyncWork<TAsyncWork>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork<TAsyncWork>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork<TAsyncWork>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task<TResult> EnqueueAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
    {
        var service = provider.GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork<TAsyncWork, TResult>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }
}

}
