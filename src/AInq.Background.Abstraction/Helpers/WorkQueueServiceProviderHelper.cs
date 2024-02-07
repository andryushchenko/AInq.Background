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

using AInq.Background.Extensions;
using AInq.Background.Services;

namespace AInq.Background.Helpers;

/// <summary> Helper class for <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> </summary>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkQueueServiceProviderHelper
{
#region Queue

    /// <inheritdoc cref="IPriorityWorkQueue.EnqueueWork(IWork,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task EnqueueWork(this IServiceProvider provider, IWork work, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>().EnqueueWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);

    /// <inheritdoc cref="IPriorityWorkQueue.EnqueueWork{TResult}(IWork{TResult},int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> EnqueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>().EnqueueWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);

    /// <inheritdoc cref="IPriorityWorkQueue.EnqueueAsyncWork(IAsyncWork,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task EnqueueAsyncWork(this IServiceProvider provider, IAsyncWork work, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>()
                      .EnqueueAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);

    /// <inheritdoc cref="IPriorityWorkQueue.EnqueueAsyncWork{TResult}(IAsyncWork{TResult},int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>()
                      .EnqueueAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, cancellation);

#endregion

#region QueueDI

    /// <inheritdoc cref="WorkQueueDependencyInjectionExtension.EnqueueWork{TWork}(IPriorityWorkQueue,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task EnqueueWork<TWork>(this IServiceProvider provider, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueWork<TWork>(priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>().EnqueueWork<TWork>(attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueDependencyInjectionExtension.EnqueueWork{TWork,TResult}(IPriorityWorkQueue,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> EnqueueWork<TWork, TResult>(this IServiceProvider provider, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueWork<TWork, TResult>(priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>().EnqueueWork<TWork, TResult>(attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueDependencyInjectionExtension.EnqueueAsyncWork{TAsyncWork}(IPriorityWorkQueue,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task EnqueueAsyncWork<TAsyncWork>(this IServiceProvider provider, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueAsyncWork<TAsyncWork>(priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork>(attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueDependencyInjectionExtension.EnqueueWork{TWork,TResult}(IPriorityWorkQueue,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityWorkQueue>()
           ?.EnqueueAsyncWork<TAsyncWork, TResult>(priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork, TResult>(attemptsCount, cancellation);

#endregion
}
