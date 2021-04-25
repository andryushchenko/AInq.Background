// Copyright 2021 Anton Andryushchenko
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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AInq.Background.Enumerable
{

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> batch processing extension </summary>
public static class WorkQueueEnumerableExtension
{
    /// <summary> Batch process works </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IEnumerable<IWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var results = (works ?? throw new ArgumentNullException(nameof(works))).Select(work => queue.EnqueueWork(work, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process works with giver <paramref name="priority" /> </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue queue, IEnumerable<IWork<TResult>> works,
        int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var results = (works ?? throw new ArgumentNullException(nameof(works))).Select(work
            => queue.EnqueueWork(work, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IWork<TResult>> works, IPriorityWorkQueue queue, int priority = 0,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works, priority, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process works using registered work queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> or <paramref name="works" /> is NULL </exception>
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).GetRequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(works, priority, cancellation, attemptsCount, enqueueAll)
            : queue.DoWorkAsync(works, cancellation, attemptsCount, enqueueAll);
    }

    /// <summary> Batch process asynchronous works </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IEnumerable<IAsyncWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var results = (works ?? throw new ArgumentNullException(nameof(works))).Select(work
            => queue.EnqueueAsyncWork(work, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IAsyncWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process works asynchronous with giver <paramref name="priority" /> </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue queue, IEnumerable<IAsyncWork<TResult>> works,
        int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var results = (works ?? throw new ArgumentNullException(nameof(works))).Select(work
            => queue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IAsyncWork<TResult>> works, IPriorityWorkQueue queue,
        int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works, priority, cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process asynchronous works using registered work queue with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no work queue is registered </exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="provider" /> or <paramref name="works" /> is NULL </exception>
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IEnumerable<IAsyncWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).GetRequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(works, priority, cancellation, attemptsCount, enqueueAll)
            : queue.DoWorkAsync(works, cancellation, attemptsCount, enqueueAll);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(queue.EnqueueWork(work, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
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

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1)
        => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works, cancellation, attemptsCount);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
        int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        var writer = channel.Writer;
        _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                        await writer.WriteAsync(queue.EnqueueWork(work, priority, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
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

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IWork<TResult>> works, IPriorityWorkQueue queue,
        int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1)
        => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works, priority, cancellation, attemptsCount);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IServiceProvider,IEnumerable{IWork{TResult}},CancellationToken,int,bool,int)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).GetRequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(works, priority, cancellation, attemptsCount)
            : queue.DoWorkAsync(works, cancellation, attemptsCount);
    }
}

}
