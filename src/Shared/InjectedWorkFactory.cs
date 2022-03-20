// Copyright 2020-2022 Anton Andryushchenko
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

using AInq.Background.Helpers;

namespace AInq.Background.Tasks;

internal static class InjectedWorkFactory
{
    public static IWork CreateInjectedWork<TWork>()
        where TWork : IWork
        => new InjectedWork<TWork>();

    public static IWork<TResult> CreateInjectedWork<TWork, TResult>()
        where TWork : IWork<TResult>
        => new InjectedWork<TWork, TResult>();

    public static IAsyncWork CreateInjectedAsyncWork<TAsyncWork>()
        where TAsyncWork : IAsyncWork
        => new InjectedAsyncWork<TAsyncWork>();

    public static IAsyncWork<TResult> CreateInjectedAsyncWork<TAsyncWork, TResult>()
        where TAsyncWork : IAsyncWork<TResult>
        => new InjectedAsyncWork<TAsyncWork, TResult>();

    private class InjectedWork<TWork> : IWork
        where TWork : IWork
    {
        void IWork.DoWork(IServiceProvider serviceProvider)
            => serviceProvider.RequiredService<TWork>().DoWork(serviceProvider);
    }

    private class InjectedWork<TWork, TResult> : IWork<TResult>
        where TWork : IWork<TResult>
    {
        TResult IWork<TResult>.DoWork(IServiceProvider serviceProvider)
            => serviceProvider.RequiredService<TWork>().DoWork(serviceProvider);
    }

    private class InjectedAsyncWork<TAsyncWork> : IAsyncWork
        where TAsyncWork : IAsyncWork
    {
        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.RequiredService<TAsyncWork>().DoWorkAsync(serviceProvider, cancellation);
    }

    private class InjectedAsyncWork<TAsyncWork, TResult> : IAsyncWork<TResult>
        where TAsyncWork : IAsyncWork<TResult>
    {
        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.RequiredService<TAsyncWork>().DoWorkAsync(serviceProvider, cancellation);
    }
}
