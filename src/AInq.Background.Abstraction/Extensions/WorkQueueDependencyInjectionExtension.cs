﻿// Copyright 2020-2021 Anton Andryushchenko
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
using AInq.Background.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Tasks.WorkFactory;

namespace AInq.Background.Extensions
{

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> extensions to enqueue work from DI </summary>
public static class WorkQueueDependencyInjectionExtension
{
#region BaseDI

    /// <summary> Enqueue background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueWork<TWork>(this IWorkQueue queue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TWork : IWork
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueWork(
            CreateWork(provider => provider.RequiredService<TWork>().DoWork(provider)),
            cancellation,
            attemptsCount);

    /// <summary> Enqueue background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IWorkQueue queue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TWork : IWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueWork(
            CreateWork(provider => provider.RequiredService<TWork>().DoWork(provider)),
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueAsyncWork<TAsyncWork>(this IWorkQueue queue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TAsyncWork : IAsyncWork
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.RequiredService<TAsyncWork>().DoWorkAsync(provider, cancel)),
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IWorkQueue queue, CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TAsyncWork : IAsyncWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.RequiredService<TAsyncWork>().DoWorkAsync(provider, cancel)),
            cancellation,
            attemptsCount);

#endregion

#region PriorityDI

    /// <summary> Enqueue background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueWork<TWork>(this IPriorityWorkQueue queue, int priority, CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TWork : IWork
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueWork(
            CreateWork(provider => provider.RequiredService<TWork>().DoWork(provider)),
            priority,
            cancellation,
            attemptsCount);

    /// <summary> Enqueue background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IPriorityWorkQueue queue, int priority, CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TWork : IWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueWork(
            CreateWork(provider => provider.RequiredService<TWork>().DoWork(provider)),
            priority,
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task EnqueueAsyncWork<TAsyncWork>(this IPriorityWorkQueue queue, int priority, CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TAsyncWork : IAsyncWork
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.RequiredService<TAsyncWork>().DoWorkAsync(provider, cancel)),
            priority,
            cancellation,
            attemptsCount);

    /// <summary> Enqueue asynchronous background work with given <paramref name="priority" /> </summary>
    /// <param name="queue"> Work queue instance </param>
    /// <param name="priority"> Work priority </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> is NULL </exception>
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IPriorityWorkQueue queue, int priority,
        CancellationToken cancellation = default, int attemptsCount = 1)
        where TAsyncWork : IAsyncWork<TResult>
        => (queue ?? throw new ArgumentNullException(nameof(queue))).EnqueueAsyncWork(
            CreateAsyncWork((provider, cancel) => provider.RequiredService<TAsyncWork>().DoWorkAsync(provider, cancel)),
            priority,
            cancellation,
            attemptsCount);

#endregion
}

}
