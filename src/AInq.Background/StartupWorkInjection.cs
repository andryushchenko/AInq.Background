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

using AInq.Background.Elements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Elements.WorkWrapperFactory;
using static AInq.Background.WorkFactory;

namespace AInq.Background
{

public static class StartupWorkInjection
{
    public static async Task DoStartupWork(this IHost host, CancellationToken cancellation = default)
    {
        using var scope = host.Services.CreateScope();
        foreach (var work in scope.ServiceProvider.GetServices<ITaskWrapper<object?>>())
        {
            using var workScope = scope.ServiceProvider.CreateScope();
            await work.ExecuteAsync(null, workScope.ServiceProvider, workScope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("Startup work"), cancellation);
        }
    }

    public static IServiceCollection AddStartupWork(this IServiceCollection services, IWork work)
        => services.AddSingleton(CreateWorkWrapper(work).Work);

    public static IServiceCollection AddStartupWork<TWork>(this IServiceCollection services)
        where TWork : IWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetService<TWork>()?.DoWork(provider))).Work);

    public static IServiceCollection AddStartupAsyncWork(this IServiceCollection services, IAsyncWork work)
        => services.AddSingleton(CreateWorkWrapper(work).Work);

    public static IServiceCollection AddStartupAsyncWork<TAsyncWork>(this IServiceCollection services)
        where TAsyncWork : IAsyncWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork((provider, token) => provider.GetService<TAsyncWork>()?.DoWorkAsync(provider, token) ?? Task.CompletedTask)).Work);

    public static IServiceCollection AddStartupQueuedWork(this IServiceCollection services, IWork work, int attemptsCount = 1)
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, attemptsCount: attemptsCount))).Work);

    public static IServiceCollection AddStartupQueuedWork<TWork>(this IServiceCollection services, int attemptsCount = 1)
        where TWork : IWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork>(attemptsCount: attemptsCount))).Work);

    public static IServiceCollection AddStartupQueuedAsyncWork(this IServiceCollection services, IAsyncWork work, int attemptsCount = 1)
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, attemptsCount: attemptsCount))).Work);

    public static IServiceCollection AddStartupQueuedAsyncWork<TAsyncWork>(this IServiceCollection services, int attemptsCount = 1)
        where TAsyncWork : IAsyncWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork>(attemptsCount: attemptsCount))).Work);
}

}
