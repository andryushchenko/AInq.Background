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

namespace AInq.Background;

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> batch processing extension </summary>
public static class WorkQueueEnumerableExtension
{
#region Enumerable

    /// <summary> Batch process works </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="works" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IEnumerable<IWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var results = (works ?? throw new ArgumentNullException(nameof(works)))
                      .Where(work => work != null)
                      .Select(work => queue.EnqueueWork(work, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process works with giver <paramref name="priority" /> </summary>
    /// <param name="priorityQueue"> Work Queue instance </param>
    /// <param name="works"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="priorityQueue" /> or <paramref name="works" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue priorityQueue, IEnumerable<IWork<TResult>> works,
        int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
        var results = (works ?? throw new ArgumentNullException(nameof(works)))
                      .Where(work => work != null)
                      .Select(work => priorityQueue.EnqueueWork(work, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IWork<TResult>> works, IPriorityWorkQueue priorityQueue,
        int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue)))
            .DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), priority, cancellation, attemptsCount, enqueueAll);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), priority, cancellation, attemptsCount, enqueueAll)
            : queue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), cancellation, attemptsCount, enqueueAll);
    }

    /// <summary> Batch process asynchronous works </summary>
    /// <param name="queue"> Work Queue instance </param>
    /// <param name="asyncWorks"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="queue" /> or <paramref name="asyncWorks" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IEnumerable<IAsyncWork<TResult>> asyncWorks,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        var results = (asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)))
                      .Where(work => work != null)
                      .Select(work => queue.EnqueueAsyncWork(work, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IAsyncWork<TResult>> asyncWorks, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process works asynchronous with giver <paramref name="priority" /> </summary>
    /// <param name="priorityQueue"> Work Queue instance </param>
    /// <param name="asyncWorks"> Works to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="priorityQueue" /> or <paramref name="asyncWorks" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue priorityQueue,
        IEnumerable<IAsyncWork<TResult>> asyncWorks, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
    {
        _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
        var results = (asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)))
                      .Where(work => work != null)
                      .Select(work => priorityQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IEnumerable<IAsyncWork<TResult>> asyncWorks, IPriorityWorkQueue priorityQueue,
        int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue)))
            .DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), priority, cancellation, attemptsCount, enqueueAll);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IEnumerable<IAsyncWork<TResult>> asyncWorks,
        CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)),
                priority,
                cancellation,
                attemptsCount,
                enqueueAll)
            : queue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), cancellation, attemptsCount, enqueueAll);
    }

#endregion

#region AsyncEnumerable

    private static async void PushWorks<TResult>(ChannelWriter<Task<TResult>> writer, IWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
        CancellationToken cancellation, int attemptsCount)
    {
        try
        {
            await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                if (work != null)
                    await writer.WriteAsync(queue.EnqueueWork(work, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
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
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        _ = works ?? throw new ArgumentNullException(nameof(works));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushWorks(channel.Writer, queue, works, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IWork<TResult>> works, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1)
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), cancellation, attemptsCount);

    private static async void PushWorks<TResult>(ChannelWriter<Task<TResult>> writer, IPriorityWorkQueue priorityQueue,
        IAsyncEnumerable<IWork<TResult>> works, int priority, CancellationToken cancellation, int attemptsCount)
    {
        try
        {
            await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                if (work != null)
                    await writer.WriteAsync(priorityQueue.EnqueueWork(work, priority, cancellation, attemptsCount), cancellation)
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
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue priorityQueue, IAsyncEnumerable<IWork<TResult>> works,
        int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
    {
        _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
        _ = works ?? throw new ArgumentNullException(nameof(works));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushWorks(channel.Writer, priorityQueue, works, priority, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IWork<TResult>> works, IPriorityWorkQueue priorityQueue,
        int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1)
        => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue)))
            .DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), priority, cancellation, attemptsCount);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IWork<TResult>> works,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), priority, cancellation, attemptsCount)
            : queue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), cancellation, attemptsCount);
    }

    private static async void PushAsyncWorks<TResult>(ChannelWriter<Task<TResult>> writer, IWorkQueue queue,
        IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, CancellationToken cancellation, int attemptsCount)
    {
        try
        {
            await foreach (var work in asyncWorks.WithCancellation(cancellation).ConfigureAwait(false))
                if (work != null)
                    await writer.WriteAsync(queue.EnqueueAsyncWork(work, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
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
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IWorkQueue queue, IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks,
        [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1)
    {
        _ = queue ?? throw new ArgumentNullException(nameof(queue));
        _ = asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushAsyncWorks(channel.Writer, queue, asyncWorks, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, IWorkQueue queue,
        CancellationToken cancellation = default, int attemptsCount = 1)
        => (queue ?? throw new ArgumentNullException(nameof(queue)))
            .DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), cancellation, attemptsCount);

    private static async void PushAsyncWorks<TResult>(ChannelWriter<Task<TResult>> writer, IPriorityWorkQueue priorityQueue,
        IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int priority, CancellationToken cancellation, int attemptsCount)
    {
        try
        {
            await foreach (var work in asyncWorks.WithCancellation(cancellation).ConfigureAwait(false))
                if (work != null)
                    await writer.WriteAsync(priorityQueue.EnqueueAsyncWork(work, priority, cancellation, attemptsCount), cancellation)
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
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IPriorityWorkQueue priorityQueue,
        IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
    {
        _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
        _ = asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushAsyncWorks(channel.Writer, priorityQueue, asyncWorks, priority, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks,
        IPriorityWorkQueue priorityQueue, int priority = 0, CancellationToken cancellation = default, int attemptsCount = 1)
        => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue)))
            .DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), priority, cancellation, attemptsCount);

    /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> DoWorkAsync<TResult>(this IServiceProvider provider, IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks,
        CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
        return queue is IPriorityWorkQueue priorityQueue
            ? priorityQueue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), priority, cancellation, attemptsCount)
            : queue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), cancellation, attemptsCount);
    }

#endregion
}
