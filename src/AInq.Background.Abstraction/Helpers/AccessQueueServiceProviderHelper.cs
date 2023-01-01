// Copyright 2020-2023 Anton Andryushchenko
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

/// <summary> Helper class for <see cref="IAccessQueue{TResource}" /> and <see cref="IPriorityAccessQueue{TResource}" /> </summary>
/// <remarks> <see cref="IAccessQueue{TResource}" /> or <see cref="IPriorityAccessQueue{TResource}" /> service should be registered on host to run queued access action </remarks>
public static class AccessQueueServiceProviderHelper
{
#region Access

    /// <inheritdoc cref="IPriorityAccessQueue{TResource}.EnqueueAccess(IAccess{TResource}, int, CancellationToken, int)" />
    [PublicAPI]
    public static Task EnqueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>()
                      .EnqueueAccess(access ?? throw new ArgumentNullException(nameof(access)), cancellation, attemptsCount);

    /// <inheritdoc cref="IPriorityAccessQueue{TResource}.EnqueueAccess{TResult}(IAccess{TResource, TResult}, int, CancellationToken, int)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>()
                      .EnqueueAccess(access ?? throw new ArgumentNullException(nameof(access)), cancellation, attemptsCount);

    /// <inheritdoc cref="IPriorityAccessQueue{TResource}.EnqueueAsyncAccess(IAsyncAccess{TResource}, int, CancellationToken, int)" />
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAsyncAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>()
                      .EnqueueAsyncAccess(access ?? throw new ArgumentNullException(nameof(access)), cancellation, attemptsCount);

    /// <inheritdoc cref="IPriorityAccessQueue{TResource}.EnqueueAsyncAccess{TResult}(IAsyncAccess{TResource, TResult}, int, CancellationToken, int)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TResult>(this IServiceProvider provider, IAsyncAccess<TResource, TResult> access,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAsyncAccess(access ?? throw new ArgumentNullException(nameof(access)), priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>()
                      .EnqueueAsyncAccess(access ?? throw new ArgumentNullException(nameof(access)), cancellation, attemptsCount);

#endregion

#region AccessDI

    /// <inheritdoc cref="AccessQueueDependencyInjectionExtension.EnqueueAccess{TResource,TAccess}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    [PublicAPI]
    public static Task EnqueueAccess<TResource, TAccess>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAccess<TResource, TAccess>(priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAccess<TResource, TAccess>(cancellation, attemptsCount);

    /// <inheritdoc cref="AccessQueueDependencyInjectionExtension.EnqueueAccess{TResource,TAccess, TResult}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAccess<TResource, TAccess, TResult>(priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAccess<TResource, TAccess, TResult>(cancellation, attemptsCount);

    /// <inheritdoc cref="AccessQueueDependencyInjectionExtension.EnqueueAsyncAccess{TResource,TAsyncAccess}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    [PublicAPI]
    public static Task EnqueueAsyncAccess<TResource, TAsyncAccess>(this IServiceProvider provider, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAsyncAccess<TResource, TAsyncAccess>(priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAsyncAccess<TResource, TAsyncAccess>(cancellation, attemptsCount);

    /// <inheritdoc cref="AccessQueueDependencyInjectionExtension.EnqueueAsyncAccess{TResource,TAsyncAccess, TResult}(IPriorityAccessQueue{TResource},int, CancellationToken, int)" />
    [PublicAPI]
    public static Task<TResult> EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityAccessQueue<TResource>>()
           ?.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IAccessQueue<TResource>>().EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(cancellation, attemptsCount);

#endregion
}
