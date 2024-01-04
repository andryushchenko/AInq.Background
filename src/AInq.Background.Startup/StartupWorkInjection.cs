// Copyright 2020-2024 Anton Andryushchenko
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
using AInq.Background.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using static AInq.Background.Tasks.QueuedWorkFactory;
using static AInq.Background.Tasks.InjectedWorkFactory;
using static AInq.Background.Wrappers.StartupWorkWrapperFactory;

namespace AInq.Background;

/// <summary> Startup work injection </summary>
public static class StartupWorkInjection
{
    /// <summary> Run registered startup works asynchronously </summary>
    /// <param name="host"> Current host </param>
    /// <param name="cancellation"> Startup work cancellation token </param>
    [PublicAPI]
    public static async Task DoStartupWorkAsync(this IHost host, CancellationToken cancellation = default)
    {
        var logger = (host ?? throw new ArgumentNullException(nameof(host))).Services.GetService<ILoggerFactory>()?.CreateLogger("Startup work")
                     ?? NullLogger.Instance;
        foreach (var work in host.Services.GetServices<IStartupWorkWrapper>())
            await work.DoWorkAsync(host.Services, logger, cancellation).ConfigureAwait(false);
    }

#region Work

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    [PublicAPI]
    public static IServiceCollection AddStartupWork(this IServiceCollection services, IWork work)
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(work ?? throw new ArgumentNullException(nameof(work))));

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupWork<TResult>(this IServiceCollection services, IWork<TResult> work)
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(work ?? throw new ArgumentNullException(nameof(work))));

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncWork(this IServiceCollection services, IAsyncWork work)
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(work ?? throw new ArgumentNullException(nameof(work))));

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncWork<TResult>(this IServiceCollection services, IAsyncWork<TResult> work)
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(work ?? throw new ArgumentNullException(nameof(work))));

#endregion

#region WorkDI

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupWork<TWork>(this IServiceCollection services)
        where TWork : IWork
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(CreateInjectedWork<TWork>()));

    /// <summary> Register startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupWork<TWork, TResult>(this IServiceCollection services)
        where TWork : IWork<TResult>
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(CreateInjectedWork<TWork, TResult>()));

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncWork<TAsyncWork>(this IServiceCollection services)
        where TAsyncWork : IAsyncWork
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(CreateInjectedAsyncWork<TAsyncWork>()));

    /// <summary> Register asynchronous startup work </summary>
    /// <param name="services"> Service collection </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncWork<TAsyncWork, TResult>(this IServiceCollection services)
        where TAsyncWork : IAsyncWork<TResult>
        => (services ?? throw new ArgumentNullException(nameof(services)))
            .AddSingleton(CreateStartupWorkWrapper(CreateInjectedAsyncWork<TAsyncWork, TResult>()));

#endregion

#region QueueWork

    /// <summary> Register queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    [PublicAPI]
    public static IServiceCollection AddStartupQueuedWork(this IServiceCollection services, IWork work, int attemptsCount = 1, int priority = 0)
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork(CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority))));

    /// <summary> Register queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupQueuedWork<TResult>(this IServiceCollection services, IWork<TResult> work, int attemptsCount = 1,
        int priority = 0)
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork<TResult>(CreateQueuedWork(work ?? throw new ArgumentNullException(nameof(work)), attemptsCount, priority))));

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="asyncWork"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncQueuedWork(this IServiceCollection services, IAsyncWork asyncWork, int attemptsCount = 1,
        int priority = 0)
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork(CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority))));

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="asyncWork"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncQueuedWork<TResult>(this IServiceCollection services, IAsyncWork<TResult> asyncWork,
        int attemptsCount = 1, int priority = 0)
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork<TResult>(CreateQueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), attemptsCount, priority))));

#endregion

#region QueueWorkDI

    /// <summary> Register queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupQueuedWork<TWork>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork(CreateQueuedInjectedWork<TWork>(attemptsCount, priority))));

    /// <summary> Register queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupQueuedWork<TWork, TResult>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork<TResult>(CreateQueuedInjectedWork<TWork, TResult>(attemptsCount, priority))));

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncQueuedWork<TAsyncWork>(this IServiceCollection services, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork(CreateQueuedInjectedAsyncWork<TAsyncWork>(attemptsCount, priority))));

    /// <summary> Register asynchronous queued startup work with given <paramref name="priority" /> (if supported) </summary>
    /// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
    /// <param name="services"> Service collection </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Work priority </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    [PublicAPI]
    public static IServiceCollection AddStartupAsyncQueuedWork<TAsyncWork, TResult>(this IServiceCollection services, int attemptsCount = 1,
        int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (services ?? throw new ArgumentNullException(nameof(services))).AddSingleton(CreateStartupWorkWrapper(
            new AsyncWork<TResult>(CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(attemptsCount, priority))));

#endregion

#region Wrapper

    private class AsyncWork : IWork
    {
        private readonly IAsyncWork _asyncWork;

        public AsyncWork(IAsyncWork asyncWork)
            => _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));

        void IWork.DoWork(IServiceProvider serviceProvider)
            => _asyncWork.DoWorkAsync(serviceProvider, CancellationToken.None);
    }

    private class AsyncWork<TResult> : IWork
    {
        private readonly IAsyncWork<TResult> _asyncWork;

        public AsyncWork(IAsyncWork<TResult> asyncWork)
            => _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));

        void IWork.DoWork(IServiceProvider serviceProvider)
            => _asyncWork.DoWorkAsync(serviceProvider, CancellationToken.None);
    }

#endregion
}
