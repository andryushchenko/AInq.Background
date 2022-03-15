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

namespace AInq.Background.Helpers;

/// <summary> Helper class for <see cref="IWorkScheduler" /> </summary>
/// <remarks> <see cref="IWorkScheduler" /> service should be registered on host to schedule work </remarks>
public static class WorkSchedulerServiceProviderHelper
{
#region Delayed

    /// <summary> Add delayed work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddScheduledWork(IWorkScheduler, IWork, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation);

    /// <summary> Add delayed work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddScheduledWork{TResult}(IWorkScheduler, IWork{TResult}, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation);

    /// <summary> Add delayed asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddScheduledAsyncWork(IWorkScheduler, IAsyncWork, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation);

    /// <summary> Add delayed asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddScheduledAsyncWork{TResult}(IWorkScheduler, IAsyncWork{TResult}, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), delay, cancellation);

#endregion

#region DelaeydDI

    /// <summary> Add delayed work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledWork{TWork}(IWorkScheduler, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork<TWork>(delay, cancellation);

    /// <summary> Add delayed work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledWork{TWork, TResult}(IWorkScheduler, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork<TWork, TResult>(delay, cancellation);

    /// <summary> Add delayed asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledAsyncWork{TAsyncWork}(IWorkScheduler, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork<TAsyncWork>(delay, cancellation);

    /// <summary> Add delayed work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Work execution delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledAsyncWork{TAsyncWork, TResult}(IWorkScheduler, TimeSpan, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork<TAsyncWork, TResult>(delay, cancellation);

#endregion

#region Scheduled

    /// <summary> Add scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork(IWork, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation);

    /// <summary> Add scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledWork{TResult}(IWork{TResult}, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation);

    /// <summary> Add scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork(IAsyncWork, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncWork(this IServiceProvider provider, IAsyncWork work, DateTime time, CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation);

    /// <summary> Add scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="IWorkScheduler.AddScheduledAsyncWork{TResult}(IAsyncWork{TResult}, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), time, cancellation);

#endregion

#region ScheduledDI

    /// <summary> Add scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledWork{TWork}(IWorkScheduler, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork<TWork>(time, cancellation);

    /// <summary> Add scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledWork{TWork, TResult}(IWorkScheduler, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledWork<TWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledWork<TWork, TResult>(time, cancellation);

    /// <summary> Add scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledAsyncWork{TAsyncWork}(IWorkScheduler, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task AddScheduledAsyncWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork<TAsyncWork>(time, cancellation);

    /// <summary> Add scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Work execution time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater than current time </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddScheduledAsyncWork{TAsyncWork, TResult}(IWorkScheduler, DateTime, CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> AddScheduledAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddScheduledAsyncWork<TAsyncWork, TResult>(time, cancellation);

#endregion

#region Cron

    /// <summary> Add CRON-scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork(IWork, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronWork(this IServiceProvider provider, IWork work, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               execCount);

    /// <summary> Add CRON-scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronWork{TResult}(IWork{TResult}, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               execCount);

    /// <summary> Add CRON-scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork(IAsyncWork, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncWork(this IServiceProvider provider, IAsyncWork work, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               execCount);

    /// <summary> Add CRON-scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="IWorkScheduler.AddCronAsyncWork{TResult}(IAsyncWork{TResult}, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncWork(work ?? throw new ArgumentNullException(nameof(work)),
               cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)),
               cancellation,
               execCount);

#endregion

#region CronDI

    /// <summary> Add CRON-scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddCronWork{TWork}(IWorkScheduler, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronWork<TWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronWork<TWork>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)), cancellation, execCount);

    /// <summary> Add CRON-scheduled work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddCronWork{TWork, TResult}(IWorkScheduler, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronWork<TWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronWork<TWork, TResult>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)), cancellation, execCount);

    /// <summary> Add CRON-scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddCronAsyncWork{TAsyncWork}(IWorkScheduler, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddCronAsyncWork<TAsyncWork>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncWork<TAsyncWork>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)), cancellation, execCount);

    /// <summary> Add CRON-scheduled asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Work CRON-based execution schedule </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work scheduler is registered </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddCronAsyncWork{TAsyncWork, TResult}(IWorkScheduler, string, CancellationToken, int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddCronAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddCronAsyncWork<TAsyncWork, TResult>(cronExpression ?? throw new ArgumentNullException(nameof(cronExpression)), cancellation, execCount);

#endregion

#region RepeatedScheduled

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddRepeatedWork(IWork,DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork(this IServiceProvider provider, IWork work, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork(work ?? throw new ArgumentNullException(nameof(work)), starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddRepeatedWork{TResult}(IWork{TResult},DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork(work ?? throw new ArgumentNullException(nameof(work)), starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddRepeatedAsyncWork(IAsyncWork,DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork(this IServiceProvider provider, IAsyncWork work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="IWorkScheduler.AddRepeatedAsyncWork{TResult}(IAsyncWork{TResult},DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), starTime, repeatDelay, cancellation, execCount);

#endregion

#region RepeatedScheduledDI

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedWork{TWork}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork<TWork>(this IServiceProvider provider, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork<TWork>(starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedWork{TWork, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TWork, TResult>(this IServiceProvider provider, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork<TWork, TResult>(starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedAsyncWork{TAsyncWork}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork<TAsyncWork>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork<TAsyncWork>(starTime, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result observable </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedAsyncWork{TAsyncWork, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork<TAsyncWork, TResult>(starTime, repeatDelay, cancellation, execCount);

#endregion

#region RepeatedDelayed

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddRepeatedWork(IWorkScheduler,IWork,TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork(this IServiceProvider provider, IWork work, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork(work ?? throw new ArgumentNullException(nameof(work)), startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddRepeatedWork{TResult}(IWorkScheduler,IWork{TResult},TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork(work ?? throw new ArgumentNullException(nameof(work)), startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddRepeatedAsyncWork(IWorkScheduler,IAsyncWork,TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork(this IServiceProvider provider, IAsyncWork work, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="work"> Work instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDelayExtension.AddRepeatedAsyncWork{TResult}(IWorkScheduler,IAsyncWork{TResult},TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork(work ?? throw new ArgumentNullException(nameof(work)), startDelay, repeatDelay, cancellation, execCount);

#endregion

#region RepeatedDelayedDI

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedWork{TWork}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedWork<TWork>(this IServiceProvider provider, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        where TWork : IWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork<TWork>(startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedWork{TWork, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedWork<TWork, TResult>(this IServiceProvider provider, TimeSpan startDelay, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
        where TWork : IWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedWork<TWork, TResult>(startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedAsyncWork{TAsyncWork}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncWork<TAsyncWork>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        where TAsyncWork : IAsyncWork
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork<TAsyncWork>(startDelay, repeatDelay, cancellation, execCount);

    /// <summary> Add repeated asynchronous work to registered scheduler </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Work first execution delay </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater than 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerDependencyInjectionExtension.AddRepeatedAsyncWork{TAsyncWork, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int)" />
    [PublicAPI]
    public static IObservable<Try<TResult>> AddRepeatedAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .RequiredService<IWorkScheduler>()
           .AddRepeatedAsyncWork<TAsyncWork, TResult>(startDelay, repeatDelay, cancellation, execCount);

#endregion
}
