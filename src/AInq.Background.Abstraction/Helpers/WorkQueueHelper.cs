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

using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Helpers
{

/// <summary> Helper class for <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> </summary>
public static class WorkQueueHelper
{
    /// <summary> Enqueue background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueWork(IWork, int, CancellationToken, int)" />
    public static Task EnqueueWork(this IServiceProvider provider, IWork work, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueWork{TWork}(int, CancellationToken, int)" />
    public static Task EnqueueWork<TWork>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TWork : IWork
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork<TWork>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork<TWork>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueWork{TResult}(IWork{TResult}, int, CancellationToken, int)" />
    public static Task<TResult> EnqueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueWork{TWork, TResult}(int, CancellationToken, int)" />
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueWork<TWork, TResult>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueWork<TWork, TResult>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue asynchronous background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueAsyncWork(IAsyncWork, int, CancellationToken, int)" />
    public static Task EnqueueAsyncWork(this IServiceProvider provider, IAsyncWork work, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue asynchronous background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueAsyncWork{TAsyncWork}(int, CancellationToken, int)" />
    public static Task EnqueueAsyncWork<TAsyncWork>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TAsyncWork : IAsyncWork
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork<TAsyncWork>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork<TAsyncWork>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue asynchronous background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueAsyncWork{TResult}(IAsyncWork{TResult}, int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork(work, cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }

    /// <summary> Enqueue asynchronous background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="IPriorityWorkQueue.EnqueueWork{TAsyncWork, TResult}(int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
    {
        var service = (provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IPriorityWorkQueue)) ?? provider.GetService(typeof(IWorkQueue));
        return service switch
        {
            IPriorityWorkQueue priorityWorkQueue => priorityWorkQueue.EnqueueAsyncWork<TAsyncWork, TResult>(priority, cancellation, attemptsCount),
            IWorkQueue workQueue => workQueue.EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, attemptsCount),
            _ => throw new InvalidOperationException("No Work Queue service found")
        };
    }
}

}
