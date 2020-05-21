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

using AInq.Support.Background.WorkElements;
using AInq.Support.Background.WorkQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Support.Background.WorkElements.WorkFactory;
using static AInq.Support.Background.WorkElements.WorkWrapperFactory;

namespace AInq.Support.Background
{
    public static class StartupWorkInjection
    {
        public static async Task DoStartupWork(this IHost host, CancellationToken cancellation = default)
        {
            using var scope = host.Services.CreateScope();
            foreach (var work in scope.ServiceProvider.GetServices<IWorkWrapper>())
            {
                using var localScope = scope.ServiceProvider.CreateScope();
                await work.DoWorkAsync(localScope.ServiceProvider, cancellation);
            }
        }

        public static IServiceCollection AddStartupWork(this IServiceCollection services, IWork work)
            => services.AddSingleton(CreateWorkWrapper(work).Work);

        public static IServiceCollection AddStartupWork<TWork>(this IServiceCollection services) where TWork : IWork
            => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetService<TWork>()?.DoWork(provider))).Work);

        public static IServiceCollection AddStartupAsyncWork(this IServiceCollection services, IAsyncWork work)
            => services.AddSingleton(CreateWorkWrapper(work).Work);

        public static IServiceCollection AddStartupAsyncWork<TWork>(this IServiceCollection services) where TWork : IAsyncWork
            => services.AddSingleton(CreateWorkWrapper(CreateWork((provider, token) => provider.GetService<TWork>()?.DoWorkAsync(provider, token) ?? Task.CompletedTask)).Work);

        public static IServiceCollection AddStartupQueuedWork(this IServiceCollection services, IWork work, int attemptsCount = 1)
            => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetService<IWorkQueue>()?.EnqueueWork(work, attemptsCount: attemptsCount))).Work);

        public static IServiceCollection AddStartupQueuedWork<TWork>(this IServiceCollection services, int attemptsCount = 1) where TWork : IWork
            => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetService<IWorkQueue>()?.EnqueueWork<TWork>(attemptsCount: attemptsCount))).Work);

        public static IServiceCollection AddStartupQueuedAsyncWork(this IServiceCollection services, IAsyncWork work, int attemptsCount = 1)
            => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetService<IWorkQueue>()?.EnqueueAsyncWork(work, attemptsCount: attemptsCount))).Work);

        public static IServiceCollection AddStartupQueuedAsyncWork<TWork>(this IServiceCollection services, int attemptsCount = 1) where TWork : IAsyncWork
            => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetService<IWorkQueue>()?.EnqueueAsyncWork<TWork>(attemptsCount: attemptsCount))).Work);
    }
}