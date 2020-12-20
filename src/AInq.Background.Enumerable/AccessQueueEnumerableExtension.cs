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

using AInq.Background.Services;
using AInq.Background.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AInq.Background.Enumerable
{

public static class AccessQueueEnumerableExtension
{
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IEnumerable<IAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0, bool enqueueAll = false)
        where TResource : notnull
    {
        var priorityAccessQueue = (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))) as IPriorityAccessQueue<TResource>;
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses)))
            .Select(access => priorityAccessQueue != null
                ? priorityAccessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount)
                : accessQueue.EnqueueAccess(access, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        bool enqueueAll = false)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, cancellation, attemptsCount, priority, enqueueAll);

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IEnumerable<IAccess<TResource, TResult>> accesses,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        where TResource : notnull
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IAccessQueue<TResource>)) as IAccessQueue<TResource>
            ?? throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found"))
            .AccessAsync(accesses, cancellation, attemptsCount, priority, enqueueAll);

    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> queue,
        IEnumerable<IAsyncAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        where TResource : notnull
    {
        var priorityAccessQueue = (queue ?? throw new ArgumentNullException(nameof(queue))) as IPriorityAccessQueue<TResource>;
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses)))
            .Select(access => priorityAccessQueue != null
                ? priorityAccessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount)
                : queue.EnqueueAsyncAccess(access, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAsyncAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> queue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        where TResource : notnull
        => queue.AccessAsync(accesses, cancellation, attemptsCount, priority, enqueueAll);

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IEnumerable<IAsyncAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        bool enqueueAll = false)
        where TResource : notnull
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IAccessQueue<TResource>)) as IAccessQueue<TResource>
            ?? throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found"))
            .AccessAsync(accesses, cancellation, attemptsCount, priority, enqueueAll);

    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var priorityAccessQueue = (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))) as IPriorityAccessQueue<TResource>;
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(priorityAccessQueue != null
                                            ? priorityAccessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount)
                                            : accessQueue.EnqueueAccess(access, cancellation, attemptsCount),
                                        cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), cancellation).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, cancellation, attemptsCount, priority);

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IAccessQueue<TResource>)) as IAccessQueue<TResource>
            ?? throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found"))
            .AccessAsync(accesses, cancellation, attemptsCount, priority);

    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var priorityAccessQueue = (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))) as IPriorityAccessQueue<TResource>;
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(priorityAccessQueue != null
                                            ? priorityAccessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount)
                                            : accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount),
                                        cancellation)
                                    .ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    await writer.WriteAsync(Task.FromCanceled<TResult>(ex.CancellationToken), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await writer.WriteAsync(Task.FromException<TResult>(ex), cancellation).ConfigureAwait(false);
                }
                finally
                {
                    writer.Complete();
                }
            },
            cancellation);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
        => accessQueue.AccessAsync(accesses, cancellation, attemptsCount, priority);

    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TResource : notnull
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IAccessQueue<TResource>)) as IAccessQueue<TResource>
            ?? throw new InvalidOperationException($"No Access Queue service for {typeof(TResource)} found"))
            .AccessAsync(accesses, cancellation, attemptsCount, priority);
}

}
