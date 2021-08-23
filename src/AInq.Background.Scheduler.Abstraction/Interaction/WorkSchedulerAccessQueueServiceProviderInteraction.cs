// Copyright 2020-2021 Anton Andryushchenko
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
using AInq.Background.Services;
using AInq.Background.Tasks;
using AInq.Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Interaction
{

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled access in registered background queue  </summary>
/// <remarks> <see cref="IWorkScheduler" /> service should be registered on host to schedule work </remarks>
/// <remarks>
///     <see cref="IPriorityAccessQueue{TResource}" /> or <see cref="IAccessQueue{TResource}" /> service should be registered on host to run queued
///     access
/// </remarks>
public static class WorkSchedulerAccessQueueServiceProviderInteraction
{
#region DelayedAccess

    /// <summary> Add delayed queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},TimeSpan,CancellationToken,int,int)" />
    public static Task AddScheduledQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess(access,
                                                                              delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TResult}(IWorkScheduler, IAccess{TResource,TResult}, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess(access,
                                                                              delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},TimeSpan,CancellationToken,int,int)" />
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess(access,
                                                                              delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TResult}(IWorkScheduler, IAsyncAccess{TResource,TResult}, TimeSpan, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess(access,
                                                                              delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region DelayedAccessDI

    /// <summary> Add delayed queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess<TResource, TAccess>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess,TResult}(IWorkScheduler,DateTime,CancellationToken,int,int)" />
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess<TResource, TAccess, TResult>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler, TimeSpan, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add delayed queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="delay"> Access execution delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="delay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess,TResult}(IWorkScheduler,DateTime,CancellationToken,int,int)" />
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider, TimeSpan delay,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(delay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region ScheduledAccess

    /// <summary> Add scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource}(IWorkScheduler, IAccess{TResource}, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess(access,
                                                                              time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TResult}(IWorkScheduler, IAccess{TResource,TResult}, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess(access,
                                                                              time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource}(IWorkScheduler, IAsyncAccess{TResource}, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess(access,
                                                                              time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TResult}(IWorkScheduler, IAsyncAccess{TResource,TResult}, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess(access,
                                                                              time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region ScheduledAccessDI

    /// <summary> Add scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledQueueAccess<TResource, TAccess>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess<TResource, TAccess>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledQueueAccess{TResource,TAccess, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledQueueAccess<TResource, TAccess, TResult>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

    /// <summary> Add scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="time"> Access execution time </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> isn't greater then current time </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddScheduledAsyncQueueAccess{TResource,TAsyncAccess, TResult}(IWorkScheduler, DateTime, CancellationToken, int, int)" />
    public static Task<TResult> AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider, DateTime time,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddScheduledAsyncQueueAccess<TResource, TAsyncAccess, TResult>(time,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority);

#endregion

#region CronAccess

    /// <summary> Add CRON-scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="access" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource}(IWorkScheduler, IAccess{TResource}, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueAccess(access,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="access" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource,TResult}(IWorkScheduler, IAccess{TResource,TResult}, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TResult>(this IServiceProvider provider, IAccess<TResource, TResult> access,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueAccess(access,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="access" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource}(IWorkScheduler, IAsyncAccess{TResource}, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueAccess(access,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="access" /> or <paramref name="cronExpression" /> or <paramref name="provider" /> is
    ///     NULL
    /// </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource,TResult}(IWorkScheduler, IAsyncAccess{TResource,TResult}, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0, int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueAccess(access,
                                                                              cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region CronAccessDI

    /// <summary> Add CRON-scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource,TAccess}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronQueueAccess<TResource, TAccess>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueAccess<TResource, TAccess>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronQueueAccess{TResource,TAccess, TResult}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, string cronExpression,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronQueueAccess<TResource, TAccess, TResult>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Maybe<Exception>> AddCronAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueAccess<TResource, TAsyncAccess>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add CRON-scheduled queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cronExpression"> Access CRON-based execution schedule </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access scheduler is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="cronExpression" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="cronExpression" /> has incorrect syntax </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddCronAsyncQueueAccess{TResource,TAsyncAccess, TResult}(IWorkScheduler, string, CancellationToken, int, int, int)" />
    public static IObservable<Try<TResult>> AddCronAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddCronAsyncQueueAccess<TResource, TAsyncAccess, TResult>(cronExpression,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedScheduledAccess

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess(access,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0,
        int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess(access,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess(access,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="starTime"> Access first execution time </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess(access,
                                                                              starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedScheduledAccessDI

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Access first execution time</param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess<TResource, TAccess>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Access first execution time</param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, DateTime starTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess<TResource, TAccess, TResult>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Access first execution time</param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="starTime"> Access first execution time</param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result observable </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess, TResult}(IWorkScheduler,DateTime,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(starTime,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedDelayedAccess

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource}(IWorkScheduler,IAccess{TResource},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource>(this IServiceProvider provider, IAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess(access,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TResult}(IWorkScheduler,IAccess{TResource,TResult},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess(access,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource}(IWorkScheduler,IAsyncAccess{TResource},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource>(this IServiceProvider provider, IAsyncAccess<TResource> access,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess(access,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="access"> Access instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> or <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TResult}(IWorkScheduler,IAsyncAccess{TResource,TResult},TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TResult>(this IServiceProvider provider,
        IAsyncAccess<TResource, TResult> access, TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess(access,
                                                                              startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion

#region RepeatedDelayedAccessDI

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedQueueAccess<TResource, TAccess>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess<TResource, TAccess>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedQueueAccess{TResource,TAccess, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedQueueAccess<TResource, TAccess, TResult>(this IServiceProvider provider, TimeSpan startDelay,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, int execCount = -1)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedQueueAccess<TResource, TAccess, TResult>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Maybe<Exception>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(this IServiceProvider provider,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

    /// <summary> Add repeated queued asynchronous access to registered scheduler with given <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="startDelay"> Access first execution delay </param>
    /// <param name="repeatDelay"> Access repeat delay </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Access priority </param>
    /// <param name="execCount"> Max access execution count (-1 for unlimited) </param>
    /// <typeparam name="TAsyncAccess"> Access type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <typeparam name="TResource"> Shared resource type</typeparam>
    /// <returns> Access result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00 </exception>
    /// <seealso
    ///     cref="WorkSchedulerAccessQueueInteraction.AddRepeatedAsyncQueueAccess{TResource,TAsyncAccess, TResult}(IWorkScheduler,TimeSpan,TimeSpan,CancellationToken,int,int,int)" />
    public static IObservable<Try<TResult>> AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(this IServiceProvider provider,
        TimeSpan startDelay, TimeSpan repeatDelay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        int execCount = -1)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                          .AddRepeatedAsyncQueueAccess<TResource, TAsyncAccess, TResult>(startDelay,
                                                                              repeatDelay,
                                                                              cancellation,
                                                                              attemptsCount,
                                                                              priority,
                                                                              execCount);

#endregion
}

}
