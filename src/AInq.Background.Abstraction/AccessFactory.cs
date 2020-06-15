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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background
{

/// <summary> Factory class for creating <see cref="IAccess{TResource}"/> and <see cref="IAsyncAccess{TResource}"/> from delegates </summary>
public static class AccessFactory
{
    private class Access<TResource> : IAccess<TResource>
    {
        private readonly Action<TResource, IServiceProvider> _access;

        internal Access(Action<TResource, IServiceProvider> access)
            => _access = access ?? throw new ArgumentNullException(nameof(access));

        void IAccess<TResource>.Access(TResource resource, IServiceProvider serviceProvider)
            => _access.Invoke(resource, serviceProvider);
    }

    private class Access<TResource, TResult> : IAccess<TResource, TResult>
    {
        private readonly Func<TResource, IServiceProvider, TResult> _access;

        internal Access(Func<TResource, IServiceProvider, TResult> access)
            => _access = access ?? throw new ArgumentNullException(nameof(access));

        TResult IAccess<TResource, TResult>.Access(TResource resource, IServiceProvider serviceProvider)
            => _access.Invoke(resource, serviceProvider);
    }

    private class AsyncAccess<TResource> : IAsyncAccess<TResource>
    {
        private readonly Func<TResource, IServiceProvider, CancellationToken, Task> _access;

        internal AsyncAccess(Func<TResource, IServiceProvider, CancellationToken, Task> access)
            => _access = access ?? throw new ArgumentNullException(nameof(access));

        async Task IAsyncAccess<TResource>.AccessAsync(TResource resource, IServiceProvider serviceProvider, CancellationToken cancellation)
            => await _access.Invoke(resource, serviceProvider, cancellation).ConfigureAwait(false);
    }

    private class AsyncAccess<TResource, TResult> : IAsyncAccess<TResource, TResult>
    {
        private readonly Func<TResource, IServiceProvider, CancellationToken, Task<TResult>> _access;

        internal AsyncAccess(Func<TResource, IServiceProvider, CancellationToken, Task<TResult>> access)
            => _access = access ?? throw new ArgumentNullException(nameof(access));

        async Task<TResult> IAsyncAccess<TResource, TResult>.AccessAsync(TResource resource, IServiceProvider serviceProvider, CancellationToken cancellation)
            => await _access.Invoke(resource, serviceProvider, cancellation).ConfigureAwait(false);
    }

    /// <summary> Creates <see cref="IAccess{TResource}"/> instance from <see cref="Action{TResource}"/> </summary>
    /// <param name="access"> Access action </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> <see cref="IAccess{TResource}"/> instance for given action </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAccess<TResource> CreateAccess<TResource>(Action<TResource> access)
        => access != null
            ? new Access<TResource>((resource, provider) => access.Invoke(resource))
            : throw new ArgumentNullException(nameof(access));

    /// <summary> Creates <see cref="IAccess{TResource, TResult}"/> instance from <see cref="Func{TResource, TResult}"/> </summary>
    /// <param name="access"> Access function </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> <see cref="IAccess{TResource, TResult}"/> instance for given function </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAccess<TResource, TResult> CreateAccess<TResource, TResult>(Func<TResource, TResult> access)
        => access != null
            ? new Access<TResource, TResult>((resource, provider) => access.Invoke(resource))
            : throw new ArgumentNullException(nameof(access));

    /// <summary> Creates <see cref="IAccess{TResource}"/> instance from <see cref="Action{TResource, IServiceProvider}"/> </summary>
    /// <param name="access"> Access action </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> <see cref="IAccess{TResource}"/> instance for given action </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAccess<TResource> CreateAccess<TResource>(Action<TResource, IServiceProvider> access)
        => new Access<TResource>(access ?? throw new ArgumentNullException(nameof(access)));

    /// <summary> Creates <see cref="IAccess{TResource, TResult}"/> instance from <see cref="Func{TResource, IServiceProvider, TResult}"/> </summary>
    /// <param name="access"> Access function </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> <see cref="IAccess{TResource, TResult}"/> instance for given function </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAccess<TResource, TResult> CreateAccess<TResource, TResult>(Func<TResource, IServiceProvider, TResult> access)
        => new Access<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)));

    /// <summary> Creates <see cref="IAsyncAccess{TResource}"/> instance from <see cref="Func{TResource, CancellationToken, Task}"/> </summary>
    /// <param name="access"> Access action </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> <see cref="IAsyncAccess{TResource}"/> instance for given action </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAsyncAccess<TResource> CreateAsyncAccess<TResource>(Func<TResource, CancellationToken, Task> access)
        => access != null
            ? new AsyncAccess<TResource>((resource, provider, token) => access.Invoke(resource, token))
            : throw new ArgumentNullException(nameof(access));

    /// <summary> Creates <see cref="IAsyncAccess{TResource, TResult}"/> instance from <see cref="Func{TResource,  CancellationToken, TResult}"/> </summary>
    /// <param name="access"> Access function </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> <see cref="IAsyncAccess{TResource, TResult}"/> instance for given function </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAsyncAccess<TResource, TResult> CreateAsyncAccess<TResource, TResult>(Func<TResource, CancellationToken, Task<TResult>> access)
        => access != null
            ? new AsyncAccess<TResource, TResult>((resource, provider, token) => access.Invoke(resource, token))
            : throw new ArgumentNullException(nameof(access));

    /// <summary> Creates <see cref="IAsyncAccess{TResource}"/> instance from <see cref="Func{TResource, IServiceProvider, CancellationToken, Task}"/> </summary>
    /// <param name="access"> Access action </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> <see cref="IAsyncAccess{TResource}"/> instance for given action </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAsyncAccess<TResource> CreateAsyncAccess<TResource>(Func<TResource, IServiceProvider, CancellationToken, Task> access)
        => new AsyncAccess<TResource>(access ?? throw new ArgumentNullException(nameof(access)));

    /// <summary> Creates <see cref="IAsyncAccess{TResource, TResult}"/> instance from <see cref="Func{TResource,  CancellationToken, IServiceProvider, TResult}"/> </summary>
    /// <param name="access"> Access function </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> <see cref="IAsyncAccess{TResource, TResult}"/> instance for given function </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="access"/> is NULL </exception>
    public static IAsyncAccess<TResource, TResult> CreateAsyncAccess<TResource, TResult>(Func<TResource, IServiceProvider, CancellationToken, Task<TResult>> access)
        => new AsyncAccess<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)));
}

}
