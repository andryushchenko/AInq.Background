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

namespace AInq.Background.Interaction;

/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
/// <remarks> <see cref="IPriorityAccessQueue{TResource}" /> or <see cref="IAccessQueue{TResource}" /> service should be registered on host to run queued access </remarks>
public static class WorkQueueAccessQueueServiceProviderInteraction
{
#region QueueAccess

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource}" />
    [PublicAPI]
    public static Task EnqueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource,TResult}(IWorkQueue,IAccess{TResource,TResult},int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource}" />
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAsyncAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource,TResult}(IWorkQueue,IAsyncAccess{TResource,TResult},int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TResult>(this IServiceProvider provider, IAsyncAccess<TResource, TResult> access,
        int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAsyncAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, attemptsCount, cancellation);

#endregion

#region QueueAccessDI

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource,TAccess}(IWorkQueue,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task EnqueueAccess<TResource, TAccess>(this IServiceProvider provider, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAccess<TResource, TAccess>(priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAccess{TResource,TAccess,TResult}" />
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAccess<TResource, TAccess, TResult>(priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource,TAsyncAccess}(IWorkQueue,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IServiceProvider provider, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAsyncAccess<TResource, TAsyncAccess>(priority, attemptsCount, cancellation);

    /// <inheritdoc cref="WorkQueueAccessQueueInteraction.EnqueueAsyncAccess{TResource,TAsyncAccess,TResult}" />
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider, int priority = 0,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkQueue>()
           .EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(priority, attemptsCount, cancellation);

#endregion
}
