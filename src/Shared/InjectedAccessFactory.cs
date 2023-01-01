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

namespace AInq.Background.Tasks;

internal static class InjectedAccessFactory
{
    public static IAccess<TResource> CreateInjectedAccess<TResource, TAccess>()
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => new InjectedAccess<TResource, TAccess>();

    public static IAccess<TResource, TResult> CreateInjectedAccess<TResource, TAccess, TResult>()
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => new InjectedAccess<TResource, TAccess, TResult>();

    public static IAsyncAccess<TResource> CreateInjectedAsyncAccess<TResource, TAsyncAccess>()
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => new InjectedAsyncAccess<TResource, TAsyncAccess>();

    public static IAsyncAccess<TResource, TResult> CreateInjectedAsyncAccess<TResource, TAsyncAccess, TResult>()
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => new InjectedAsyncAccess<TResource, TAsyncAccess, TResult>();

    private class InjectedAccess<TResource, TAccess> : IAccess<TResource>
        where TResource : notnull
        where TAccess : IAccess<TResource>
    {
        void IAccess<TResource>.Access(TResource resource, IServiceProvider serviceProvider)
            => serviceProvider.RequiredService<TAccess>().Access(resource, serviceProvider);
    }

    private class InjectedAccess<TResource, TAccess, TResult> : IAccess<TResource, TResult>
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
    {
        TResult IAccess<TResource, TResult>.Access(TResource resource, IServiceProvider serviceProvider)
            => serviceProvider.RequiredService<TAccess>().Access(resource, serviceProvider);
    }

    private class InjectedAsyncAccess<TResource, TAsyncAccess> : IAsyncAccess<TResource>
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
    {
        Task IAsyncAccess<TResource>.AccessAsync(TResource resource, IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.RequiredService<TAsyncAccess>().AccessAsync(resource, serviceProvider, cancellation);
    }

    private class InjectedAsyncAccess<TResource, TAsyncAccess, TResult> : IAsyncAccess<TResource, TResult>
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
    {
        Task<TResult> IAsyncAccess<TResource, TResult>.AccessAsync(TResource resource, IServiceProvider serviceProvider,
            CancellationToken cancellation)
            => serviceProvider.RequiredService<TAsyncAccess>().AccessAsync(resource, serviceProvider, cancellation);
    }
}
