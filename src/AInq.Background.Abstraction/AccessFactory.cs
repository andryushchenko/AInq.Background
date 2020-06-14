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
            => await _access.Invoke(resource, serviceProvider, cancellation);
    }

    private class AsyncAccess<TResource, TResult> : IAsyncAccess<TResource, TResult>
    {
        private readonly Func<TResource, IServiceProvider, CancellationToken, Task<TResult>> _access;

        internal AsyncAccess(Func<TResource, IServiceProvider, CancellationToken, Task<TResult>> access)
            => _access = access ?? throw new ArgumentNullException(nameof(access));

        async Task<TResult> IAsyncAccess<TResource, TResult>.AccessAsync(TResource resource, IServiceProvider serviceProvider, CancellationToken cancellation)
            => await _access.Invoke(resource, serviceProvider, cancellation);
    }

    public static IAccess<TResource> CreateAccess<TResource>(Action<TResource> access)
        => access != null
            ? new Access<TResource>((resource, provider) => access.Invoke(resource))
            : throw new ArgumentNullException(nameof(access));

    public static IAccess<TResource, TResult> CreateAccess<TResource, TResult>(Func<TResource, TResult> access)
        => access != null
            ? new Access<TResource, TResult>((resource, provider) => access.Invoke(resource))
            : throw new ArgumentNullException(nameof(access));

    public static IAccess<TResource> CreateAccess<TResource>(Action<TResource, IServiceProvider> access)
        => new Access<TResource>(access ?? throw new ArgumentNullException(nameof(access)));

    public static IAccess<TResource, TResult> CreateAccess<TResource, TResult>(Func<TResource, IServiceProvider, TResult> access)
        => new Access<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)));

    public static IAsyncAccess<TResource> CreateAsyncAccess<TResource>(Func<TResource, CancellationToken, Task> access)
        => access != null
            ? new AsyncAccess<TResource>((resource, provider, token) => access.Invoke(resource, token))
            : throw new ArgumentNullException(nameof(access));

    public static IAsyncAccess<TResource, TResult> CreateAsyncAccess<TResource, TResult>(Func<TResource, CancellationToken, Task<TResult>> access)
        => access != null
            ? new AsyncAccess<TResource, TResult>((resource, provider, token) => access.Invoke(resource, token))
            : throw new ArgumentNullException(nameof(access));

    public static IAsyncAccess<TResource> CreateAsyncAccess<TResource>(Func<TResource, IServiceProvider, CancellationToken, Task> access)
        => new AsyncAccess<TResource>(access ?? throw new ArgumentNullException(nameof(access)));

    public static IAsyncAccess<TResource, TResult> CreateAsyncAccess<TResource, TResult>(Func<TResource, IServiceProvider, CancellationToken, Task<TResult>> access)
        => new AsyncAccess<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)));
}

}
