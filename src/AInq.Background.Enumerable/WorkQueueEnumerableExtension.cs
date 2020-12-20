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

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> batch processing extension </summary>
public static class WorkQueueEnumerableExtension
{
    /// <summary> Batch process works with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    /// <seealso cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData, int, CancellationToken, int)" />
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IEnumerable<IWork<TResult>> works,
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

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => queue.DoWorkAsync(works, cancellation, attemptsCount, priority, enqueueAll);

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
    /// <seealso cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData, int, CancellationToken, int)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorkAsync(works, cancellation, attemptsCount, priority, enqueueAll);

    /// <summary> Batch process asynchronous works with giver <paramref name="priority" /> (if supported) </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    /// <seealso cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData, int, CancellationToken, int)" />
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IEnumerable<IAsyncWork<TResult>> works,
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

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IAsyncWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => queue.DoWorkAsync(works, cancellation, attemptsCount, priority, enqueueAll);

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
    /// <seealso cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData, int, CancellationToken, int)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IEnumerable<IAsyncWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0, bool enqueueAll = false)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorkAsync(works, cancellation, attemptsCount, priority, enqueueAll);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,int,bool)" />
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
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

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => queue.DoWorkAsync(works, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IServiceProvider,IEnumerable{IWork{TResult}},CancellationToken,int,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorkAsync(works, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,int,bool)" />
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IAsyncWork<TResult>> works,
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

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IAsyncWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => queue.DoWorkAsync(works, cancellation, attemptsCount, priority);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IServiceProvider,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,int,bool)" />
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IAsyncWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => ((provider ?? throw new ArgumentNullException(nameof(provider))).GetService(typeof(IWorkQueue)) as IWorkQueue
            ?? throw new InvalidOperationException("No Work Queue service found"))
            .DoWorkAsync(works, cancellation, attemptsCount, priority);
}

}
