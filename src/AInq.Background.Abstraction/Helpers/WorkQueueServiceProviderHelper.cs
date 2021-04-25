// Copyright 2021 Anton Andryushchenko
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

using AInq.Background.Extensions;
using AInq.Background.Services;
using AInq.Background.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Helpers
{

/// <summary> Helper class for <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> </summary>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkQueueServiceProviderHelper
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
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueWork(work, priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, attemptsCount);

    /// <summary> Enqueue background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueDependencyInjectionExtension.EnqueueWork{TWork}(IPriorityWorkQueue,int, CancellationToken, int)" />
    public static Task EnqueueWork<TWork>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueWork<TWork>(priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork>(cancellation, attemptsCount);

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
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueWork(work, priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, attemptsCount);

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
    /// <seealso cref="WorkQueueDependencyInjectionExtension.EnqueueWork{TWork, TResult}(IPriorityWorkQueue,int, CancellationToken, int)" />
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueWork<TWork, TResult>(priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork, TResult>(cancellation, attemptsCount);

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
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueAsyncWork(work, priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, attemptsCount);

    /// <summary> Enqueue asynchronous background work into registered queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <seealso cref="WorkQueueDependencyInjectionExtension.EnqueueAsyncWork{TAsyncWork}(IPriorityWorkQueue,int, CancellationToken, int)" />
    public static Task EnqueueAsyncWork<TAsyncWork>(this IServiceProvider provider, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueAsyncWork<TAsyncWork>(priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork>(cancellation, attemptsCount);

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
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueAsyncWork(work, priority, cancellation, attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, attemptsCount);

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
    /// <seealso cref="WorkQueueDependencyInjectionExtension.EnqueueWork{TAsyncWork, TResult}(IPriorityWorkQueue,int, CancellationToken, int)" />
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).GetService<IPriorityWorkQueue>()
                                                                          ?.EnqueueAsyncWork<TAsyncWork, TResult>(priority,
                                                                              cancellation,
                                                                              attemptsCount)
           ?? provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, attemptsCount);
}

}
