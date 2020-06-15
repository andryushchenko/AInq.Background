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

using AInq.Background.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.WorkFactory;
using static AInq.Background.Wrappers.WorkWrapperFactory;

namespace AInq.Background
{

/// <summary> Startup work injection </summary>
public static class StartupWorkInjection
{
    /// <summary> Run registered startup works asynchronously </summary>
    /// <param name="host"> Current host </param>
    /// <param name="cancellation"> Startup work cancellation token </param>
    /// <returns></returns>
    public static async Task DoStartupWork(this IHost host, CancellationToken cancellation = default)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("Startup work");
        foreach (var work in scope.ServiceProvider.GetServices<ITaskWrapper<object?>>())
        {
            using var workScope = scope.ServiceProvider.CreateScope();
            await work.ExecuteAsync(null, workScope.ServiceProvider, logger, cancellation);
        }
    }

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    public static IServiceCollection AddStartupWork(this IServiceCollection services, IWork work)
        => services.AddSingleton(CreateWorkWrapper(work).Work);

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    public static IServiceCollection AddStartupWork<TWork>(this IServiceCollection services)
        where TWork : IWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider))).Work);

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupWork<TResult>(this IServiceCollection services, IWork<TResult> work)
        => services.AddSingleton(CreateWorkWrapper(work).Work);

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupWork<TWork, TResult>(this IServiceCollection services)
        where TWork : IWork<TResult>
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider))).Work);

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    public static IServiceCollection AddStartupAsyncWork(this IServiceCollection services, IAsyncWork work)
        => services.AddSingleton(CreateWorkWrapper(work).Work);

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    public static IServiceCollection AddStartupAsyncWork<TAsyncWork>(this IServiceCollection services)
        where TAsyncWork : IAsyncWork
        => services.AddSingleton(CreateWorkWrapper(CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token))).Work);

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    public static IServiceCollection AddStartupAsyncWork<TResult>(this IServiceCollection services, IAsyncWork<TResult> work)
        => services.AddSingleton(CreateWorkWrapper(work).Work);

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupAsyncWork<TAsyncWork, TResult>(this IServiceCollection services)
        where TAsyncWork : IAsyncWork<TResult>
        => services.AddSingleton(CreateWorkWrapper(CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token))).Work);

    /// <summary> Register queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    public static IServiceCollection AddStartupQueuedWork(this IServiceCollection services, IWork work, int attemptsCount = 1, int priority = 0)
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueWork(work, CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    public static IServiceCollection AddStartupQueuedWork<TWork>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueWork<TWork>(CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupQueuedWork<TResult>(this IServiceCollection services, IWork<TResult> work, int attemptsCount = 1, int priority = 0)
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueWork(work, CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupQueuedWork<TWork, TResult>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueWork<TWork, TResult>(CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    public static IServiceCollection AddStartupAsyncQueuedWork(this IServiceCollection services, IAsyncWork work, int attemptsCount = 1, int priority = 0)
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueAsyncWork(work, CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    public static IServiceCollection AddStartupAsyncQueuedWork<TAsyncWork>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork>(CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupAsyncQueuedWork<TResult>(this IServiceCollection services, IAsyncWork<TResult> work, int attemptsCount = 1, int priority = 0)
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueAsyncWork(work, CancellationToken.None, attemptsCount, priority).Ignore())).Work);

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority"/> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue"/> or <see cref="IWorkQueue"/> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    public static IServiceCollection AddStartupAsyncQueuedWork<TAsyncWork, TResult>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => services.AddSingleton(CreateWorkWrapper(CreateWork(provider => provider.EnqueueAsyncWork<TAsyncWork, TResult>(CancellationToken.None, attemptsCount, priority).Ignore())).Work);
}

}
