// Copyright 2020-2023 Anton Andryushchenko
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
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IEnumerable<IAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default, int attemptsCount = 1,
        bool enqueueAll = false)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses)))
                      .Where(access => access != null)
                      .Select(access => accessQueue.EnqueueAccess(access, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue)))
            .AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process access actions with giver <paramref name="priority" /> </summary>
    /// <param name="priorityAccessQueue"> Access Queue instance </param>
    /// <param name="accesses"> Access actions to process </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="priorityAccessQueue" /> or <paramref name="accesses" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> priorityAccessQueue,
        IEnumerable<IAccess<TResource, TResult>> accesses, int priority = 0, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
    {
        _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
        var results = (accesses ?? throw new ArgumentNullException(nameof(accesses)))
                      .Where(access => access != null)
                      .Select(access => priorityAccessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAccess<TResource, TResult>> accesses,
        IPriorityAccessQueue<TResource> priorityAccessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        bool enqueueAll = false)
        where TResource : notnull
        => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue)))
            .AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), priority, cancellation, attemptsCount, enqueueAll);

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IEnumerable<IAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false,
        int priority = 0)
        where TResource : notnull
    {
        var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
            ? priorityAccessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)),
                priority,
                cancellation,
                attemptsCount,
                enqueueAll)
            : accessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), cancellation, attemptsCount, enqueueAll);
    }

    /// <summary> Batch process asynchronous access actions </summary>
    /// <param name="accessQueue"> Access Queue instance </param>
    /// <param name="asyncAccesses"> Access actions to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="accessQueue" /> or <paramref name="asyncAccesses" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        var results = (asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)))
                      .Where(access => access != null)
                      .Select(access => accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue)))
            .AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)), cancellation, attemptsCount, enqueueAll);

    /// <summary> Batch process asynchronous access actions with giver <paramref name="priority" /> </summary>
    /// <param name="priorityAccessQueue"> Access Queue instance </param>
    /// <param name="asyncAccesses"> Access actions to process </param>
    /// <param name="priority"> Operation priority </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="enqueueAll"> Option to enqueue all data first </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task enumeration </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="priorityAccessQueue" /> or <paramref name="asyncAccesses" /> is NULL </exception>
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> priorityAccessQueue,
        IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int priority = 0,
        [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1, bool enqueueAll = false)
        where TResource : notnull
    {
        _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
        var results = (asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)))
                      .Where(access => access != null)
                      .Select(access => priorityAccessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount));
        if (enqueueAll) results = results.ToList();
        foreach (var result in results)
            yield return await result.ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
        IPriorityAccessQueue<TResource> priorityAccessQueue, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0,
        bool enqueueAll = false)
        where TResource : notnull
        => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue)))
            .AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)), priority, cancellation, attemptsCount, enqueueAll);

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, CancellationToken cancellation = default, int attemptsCount = 1,
        bool enqueueAll = false, int priority = 0)
        where TResource : notnull
    {
        var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
            ? priorityAccessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                priority,
                cancellation,
                attemptsCount,
                enqueueAll)
            : accessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                cancellation,
                attemptsCount,
                enqueueAll);
    }

#endregion

#region AsyncEnumerable

    private static async void PushAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer, IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, CancellationToken cancellation, int attemptsCount)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                if (access != null)
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
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        _ = accesses ?? throw new ArgumentNullException(nameof(accesses));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushAccesses(channel.Writer, accessQueue, accesses, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAccess<TResource, TResult>> accesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue)))
            .AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), cancellation, attemptsCount);

    private static async void PushAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer,
        IPriorityAccessQueue<TResource> priorityAccessQueue, IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int priority,
        CancellationToken cancellation, int attemptsCount)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                if (access != null)
                    await writer.WriteAsync(priorityAccessQueue.EnqueueAccess(access, priority, cancellation, attemptsCount), cancellation)
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

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> priorityAccessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int priority, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
        _ = accesses ?? throw new ArgumentNullException(nameof(accesses));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushAccesses(channel.Writer, priorityAccessQueue, accesses, priority, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAccess<TResource, TResult>> accesses,
        IPriorityAccessQueue<TResource> priorityAccessQueue, int priority, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue)))
            .AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), priority, cancellation, attemptsCount);

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TResource : notnull
    {
        var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
            ? priorityAccessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), priority, cancellation, attemptsCount)
            : accessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), cancellation, attemptsCount);
    }

    private static async void PushAsyncAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer, IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, CancellationToken cancellation, int attemptsCount)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in asyncAccesses.WithCancellation(cancellation).ConfigureAwait(false))
                if (access != null)
                    await writer.WriteAsync(accessQueue.EnqueueAsyncAccess(access, cancellation, attemptsCount), cancellation).ConfigureAwait(false);
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

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
        _ = asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushAsyncAccesses(channel.Writer, accessQueue, asyncAccesses, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
        IAccessQueue<TResource> accessQueue, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue)))
            .AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)), cancellation, attemptsCount);

    private static async void PushAsyncAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer,
        IPriorityAccessQueue<TResource> priorityAccessQueue, IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int priority,
        CancellationToken cancellation, int attemptsCount)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in asyncAccesses.WithCancellation(cancellation).ConfigureAwait(false))
                if (access != null)
                    await writer.WriteAsync(priorityAccessQueue.EnqueueAsyncAccess(access, priority, cancellation, attemptsCount), cancellation)
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

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static async IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IPriorityAccessQueue<TResource> priorityAccessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int priority,
        [EnumeratorCancellation] CancellationToken cancellation = default,
        int attemptsCount = 1)
        where TResource : notnull
    {
        _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
        _ = asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses));
        var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
        var reader = channel.Reader;
        PushAsyncAccesses(channel.Writer, priorityAccessQueue, asyncAccesses, priority, cancellation, attemptsCount);
        while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
            yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
        IPriorityAccessQueue<TResource> priorityAccessQueue, int priority, CancellationToken cancellation = default, int attemptsCount = 1)
        where TResource : notnull
        => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue)))
            .AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)), priority, cancellation, attemptsCount);

    /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,CancellationToken,int,bool)" />
    [PublicAPI]
    public static IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(this IServiceProvider provider,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, CancellationToken cancellation = default, int attemptsCount = 1,
        int priority = 0)
        where TResource : notnull
    {
        var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
        return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
            ? priorityAccessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                priority,
                cancellation,
                attemptsCount)
            : accessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)), cancellation, attemptsCount);
    }

#endregion
}
