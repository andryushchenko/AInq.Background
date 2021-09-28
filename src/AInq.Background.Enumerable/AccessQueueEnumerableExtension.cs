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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AInq.Background
{

/// <summary> <see cref="IPriorityAccessQueue{TResource}" /> and <see cref="IAccessQueue{TResource}" /> batch processing extension </summary>
public static class AccessQueueEnumerableExtension
{
#region Enumerable

    /// <summary> Batch process access actions </summary>
    /// <param name="accessQueue"> Access Queue instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="accesses" /> or <paramref name="accesses" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IEnumerable<IAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1,
        bool enqueueAll = false)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses))).Select(access
            => accessQueue.EnqueueAccess(access, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(accesses, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process access actions with giver <paramref name="priority" /> </summary>
    /// <param name="accessQueue"> Access Queue instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="accessQueue" /> or <paramref name="accesses" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> accessQueue,
        IEnumerable<IAccess<TResource, TResult>> accesses, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses))).Select(access
            => accessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAccess<TResource, TResult>> accesses,
        IPriorityAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        bool enqueueAll = false)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(accesses,
            priority,
            cancellation,
            attemptsCount,
            enqueueAll);

    /// <summary> Batch process access actions using registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> or <paramref name="accesses" /> is NULL </exception>
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IEnumerable<IAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false,
        int priority = 0)
        where TResource : notnull
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return queue is IPriorityAccessQueue<TResource> priorityQueue
            ? priorityQueue.AccessAsync(accesses, priority, cancellation, attemptsCount, enqueueAll)
            : queue.AccessAsync(accesses, cancellation, attemptsCount, enqueueAll);
    }

    /// <summary> Batch process asynchronous access actions </summary>
    /// <param name="accessQueue"> Access Queue instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="accessQueue" /> or <paramref name="accesses" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IEnumerable<IAsyncAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses))).Select(access
            => accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAsyncAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(accesses, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process asynchronous access actions with giver <paramref name="priority" /> </summary>
    /// <param name="accessQueue"> Access Queue instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="accessQueue" /> or <paramref name="accesses" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> accessQueue,
        IEnumerable<IAsyncAccess<TResource, TResult>> accesses, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses))).Select(access
            => accessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAsyncAccess<TResource, TResult>> accesses,
        IPriorityAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        bool enqueueAll = false)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(accesses,
            priority,
            cancellation,
            attemptsCount,
            enqueueAll);

    /// <summary> Batch process asynchronous access actions using registered access queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no access queue for <typeparamref name="TResource" /> is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> or <paramref name="accesses" /> is NULL </exception>
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IEnumerable<IAsyncAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1,
        bool enqueueAll = false, int priority = 0)
        where TResource : notnull
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return queue is IPriorityAccessQueue<TResource> priorityQueue
            ? priorityQueue.AccessAsync(accesses, priority, cancellation, attemptsCount, enqueueAll)
            : queue.AccessAsync(accesses, cancellation, attemptsCount, enqueueAll);
    }

#endregion

#region AsyncEnumerable

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(accessQueue.EnqueueAccess(access, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
#if NET5_0_OR_GREATER
        await foreach (var result in reader.ReadAllAsync(cancellation).ConfigureAwait(false))
            yield return await result.ConfigureAwait(false);
#else
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, cancellation, attemptsCount);

    /// <inheritdoc
    ///     cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int priority, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(accessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount), cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
#if NET5_0_OR_GREATER
        await foreach (var result in reader.ReadAllAsync(cancellation).ConfigureAwait(false))
            yield return await result.ConfigureAwait(false);
#else
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc
    ///     cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAccess<TResource, TResult>> accesses,
        IPriorityAccessQueue<TResource> accessQueue, int priority, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, priority, cancellation, attemptsCount);

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IServiceProvider,IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool,int)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return queue is IPriorityAccessQueue<TResource> priorityQueue
            ? priorityQueue.AccessAsync(accesses, priority, cancellation, attemptsCount)
            : queue.AccessAsync(accesses, cancellation, attemptsCount);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount), cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
#if NET5_0_OR_GREATER
        await foreach (var result in reader.ReadAllAsync(cancellation).ConfigureAwait(false))
            yield return await result.ConfigureAwait(false);
#else
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, cancellation, attemptsCount);

    /// <inheritdoc
    ///     cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses, int priority, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(accessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount), cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
#if NET5_0_OR_GREATER
        await foreach (var result in reader.ReadAllAsync(cancellation).ConfigureAwait(false))
            yield return await result.ConfigureAwait(false);
#else
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc
    ///     cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses,
        IPriorityAccessQueue<TResource> accessQueue, int priority, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, priority, cancellation, attemptsCount);

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IServiceProvider,IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool,int)" />
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TResource : notnull
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return queue is IPriorityAccessQueue<TResource> priorityQueue
            ? priorityQueue.AccessAsync(accesses, priority, cancellation, attemptsCount)
            : queue.AccessAsync(accesses, cancellation, attemptsCount);
    }

#endregion
}

}
