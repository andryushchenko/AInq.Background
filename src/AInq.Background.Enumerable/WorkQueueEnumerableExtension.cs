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

public static class WorkQueueEnumerableExtension
{
    public static async IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IWorkQueue queue, IEnumerable<IWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
    {
        var priorityQueue = (queue ?? throw new ArgumentNullException(nameof(queue))) as IPriorityWorkQueue;
        var results = (works ?? throw new ArgumentNullException(nameof(works)))
            .Select(work => priorityQueue != null
                ? priorityQueue.EnqueueWork(work, priority, cancellation, attemptsCount)
                : queue.EnqueueWork(work, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => queue.DoWorksAsync(works, cancellation, attemptsCount, priority, enqueueAll);

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IServiceProvider provider, IEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorksAsync(works, cancellation, attemptsCount, priority, enqueueAll);

    public static async IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IWorkQueue queue, IEnumerable<IAsyncWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
    {
        var priorityQueue = (queue ?? throw new ArgumentNullException(nameof(queue))) as IPriorityWorkQueue;
        var results = (works ?? throw new ArgumentNullException(nameof(works)))
            .Select(work => priorityQueue != null
                ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount)
                : queue.EnqueueAsyncWork(work, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IEnumerable<IAsyncWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => queue.DoWorksAsync(works, cancellation, attemptsCount, priority, enqueueAll);

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IServiceProvider provider, IEnumerable<IAsyncWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorksAsync(works, cancellation, attemptsCount, priority, enqueueAll);

    public static async IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var priorityQueue = (queue ?? throw new ArgumentNullException(nameof(queue))) as IPriorityWorkQueue;
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(priorityQueue != null
                                            ? priorityQueue.EnqueueWork(work, priority, cancellation, attemptsCount)
                                            : queue.EnqueueWork(work, cancellation, attemptsCount),
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

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IAsyncEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => queue.DoWorksAsync(works, cancellation, attemptsCount, priority);

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorksAsync(works, cancellation, attemptsCount, priority);

    public static async IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IAsyncWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var priorityQueue = (queue ?? throw new ArgumentNullException(nameof(queue))) as IPriorityWorkQueue;
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(priorityQueue != null
                                            ? priorityQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount)
                                            : queue.EnqueueAsyncWork(work, cancellation, attemptsCount),
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

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IAsyncEnumerable<IAsyncWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => queue.DoWorksAsync(works, cancellation, attemptsCount, priority);

    public static IAsyncEnumerable<TResult> DoWorksAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IAsyncWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorksAsync(works, cancellation, attemptsCount, priority);
}

}
