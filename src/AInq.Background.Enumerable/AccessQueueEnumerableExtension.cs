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

namespace AInq.Background;

/// <summary> <see cref="IPriorityAccessQueue{TResource}" /> and <see cref="IAccessQueue{TResource}" /> batch processing extension </summary>
public static class AccessQueueEnumerableExtension
{
    private static async void PushAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer,
        IPriorityAccessQueue<TResource> priorityAccessQueue, IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int priority, int attemptsCount,
        CancellationToken cancellation)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(priorityAccessQueue.EnqueueAccess(access, priority, attemptsCount, cancellation), cancellation)
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

    private static async void PushAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer, IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int attemptsCount, CancellationToken cancellation)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in accesses.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(accessQueue.EnqueueAccess(access, attemptsCount, cancellation), cancellation).ConfigureAwait(false);
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

    private static async void PushAsyncAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer, IAccessQueue<TResource> accessQueue,
        IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int attemptsCount, CancellationToken cancellation)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in asyncAccesses.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(accessQueue.EnqueueAsyncAccess(access, attemptsCount, cancellation), cancellation).ConfigureAwait(false);
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

    private static async void PushAsyncAccesses<TResource, TResult>(ChannelWriter<Task<TResult>> writer,
        IPriorityAccessQueue<TResource> priorityAccessQueue, IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int priority,
        int attemptsCount, CancellationToken cancellation)
        where TResource : notnull
    {
        try
        {
            await foreach (var access in asyncAccesses.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(priorityAccessQueue.EnqueueAsyncAccess(access, priority, attemptsCount, cancellation), cancellation)
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

    /// <param name="accessQueue"> Access Queue instance </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    extension<TResource>(IAccessQueue<TResource> accessQueue)
        where TResource : notnull
    {
        /// <summary> Batch process access actions </summary>
        /// <param name="accesses"> Access actions to be processed </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IEnumerable<IAccess<TResource, TResult>> accesses, int attemptsCount = 1,
            bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
            var results = (accesses ?? throw new ArgumentNullException(nameof(accesses))).Select(access
                => accessQueue.EnqueueAccess(access, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <summary> Batch process asynchronous access actions </summary>
        /// <param name="asyncAccesses"> Access actions to be processed </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
            int attemptsCount = 1, bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)

        {
            _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
            var results = (asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses))).Select(access
                => accessQueue.EnqueueAsyncAccess(access, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int attemptsCount = 1,
            [EnumeratorCancellation] CancellationToken cancellation = default)

        {
            _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
            _ = accesses ?? throw new ArgumentNullException(nameof(accesses));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushAccesses(channel.Writer, accessQueue, accesses, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="AccessQueueEnumerableExtension.AccessAsync{TResource,TResult}(AInq.Background.Services.IAccessQueue{TResource},System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAsyncAccess{TResource,TResult}},int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
            int attemptsCount = 1, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = accessQueue ?? throw new ArgumentNullException(nameof(accessQueue));
            _ = asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushAsyncAccesses(channel.Writer, accessQueue, asyncAccesses, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }

    /// <param name="priorityAccessQueue"> Access Queue instance </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    extension<TResource>(IPriorityAccessQueue<TResource> priorityAccessQueue)
        where TResource : notnull
    {
        /// <summary> Batch process access actions with giver <paramref name="priority" /> </summary>
        /// <param name="accesses"> Access actions to be processed </param>
        /// <param name="priority"> Operation priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IEnumerable<IAccess<TResource, TResult>> accesses, int priority = 0,
            int attemptsCount = 1, bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
            var results = (accesses ?? throw new ArgumentNullException(nameof(accesses))).Select(access
                => priorityAccessQueue.EnqueueAccess(access, priority, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <summary> Batch process asynchronous access actions with giver <paramref name="priority" /> </summary>
        /// <param name="asyncAccesses"> Access actions to be processed </param>
        /// <param name="priority"> Operation priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int priority = 0,
            int attemptsCount = 1, bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)

        {
            _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
            var results = (asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses))).Select(access
                => priorityAccessQueue.EnqueueAsyncAccess(access, priority, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <inheritdoc cref="AccessQueueEnumerableExtension.AccessAsync{TResource,TResult}(AInq.Background.Services.IPriorityAccessQueue{TResource},System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAccess{TResource,TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int priority,
            int attemptsCount = 1, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
            _ = accesses ?? throw new ArgumentNullException(nameof(accesses));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushAccesses(channel.Writer, priorityAccessQueue, accesses, priority, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="AccessQueueEnumerableExtension.AccessAsync{TResource,TResult}(AInq.Background.Services.IPriorityAccessQueue{TResource},System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAsyncAccess{TResource,TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> AccessAsync<TResult>(IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses, int priority,
            int attemptsCount = 1, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue));
            _ = asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushAsyncAccesses(channel.Writer, priorityAccessQueue, asyncAccesses, priority, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }

    /// <param name="accesses"> Access actions to be processed </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResource, TResult>(IEnumerable<IAccess<TResource, TResult>> accesses)
        where TResource : notnull
    {
        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IAccessQueue<TResource> accessQueue, int attemptsCount = 1, bool enqueueAll = false,
            CancellationToken cancellation = default)
            => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(
                accesses ?? throw new ArgumentNullException(nameof(accesses)),
                attemptsCount,
                enqueueAll,
                cancellation);

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IPriorityAccessQueue<TResource> priorityAccessQueue, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, CancellationToken cancellation = default)
            => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue))).AccessAsync(
                accesses ?? throw new ArgumentNullException(nameof(accesses)),
                priority,
                attemptsCount,
                enqueueAll,
                cancellation);
    }

    /// <param name="asyncAccesses"> Access actions to be processed </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResource, TResult>(IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses)
        where TResource : notnull
    {
        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IAccessQueue<TResource> accessQueue, int attemptsCount = 1, bool enqueueAll = false,
            CancellationToken cancellation = default)
            => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(
                asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                attemptsCount,
                enqueueAll,
                cancellation);

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IPriorityAccessQueue<TResource> priorityAccessQueue, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, CancellationToken cancellation = default)
            => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue))).AccessAsync(
                asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                priority,
                attemptsCount,
                enqueueAll,
                cancellation);
    }

    /// <param name="accesses"> Access actions to be processed </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResource, TResult>(IAsyncEnumerable<IAccess<TResource, TResult>> accesses)
        where TResource : notnull
    {
        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IAccessQueue<TResource> accessQueue, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(
                accesses ?? throw new ArgumentNullException(nameof(accesses)),
                attemptsCount,
                cancellation);

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IPriorityAccessQueue<TResource> priorityAccessQueue, int priority, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue))).AccessAsync(
                accesses ?? throw new ArgumentNullException(nameof(accesses)),
                priority,
                attemptsCount,
                cancellation);
    }

    /// <param name="asyncAccesses"> Access actions to be processed </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResource, TResult>(IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses)
        where TResource : notnull
    {
        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IAccessQueue<TResource> accessQueue, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (accessQueue ?? throw new ArgumentNullException(nameof(accessQueue))).AccessAsync(
                asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                attemptsCount,
                cancellation);

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync(IPriorityAccessQueue<TResource> priorityAccessQueue, int priority, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (priorityAccessQueue ?? throw new ArgumentNullException(nameof(priorityAccessQueue))).AccessAsync(
                asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                priority,
                attemptsCount,
                cancellation);
    }

    /// <param name="provider"> Service provider instance </param>
    extension(IServiceProvider provider)
    {
        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(IEnumerable<IAccess<TResource, TResult>> accesses, int priority = 0,
            int attemptsCount = 1, bool enqueueAll = false, CancellationToken cancellation = default)
            where TResource : notnull
        {
            var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
            return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
                ? priorityAccessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)),
                    priority,
                    attemptsCount,
                    enqueueAll,
                    cancellation)
                : accessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), attemptsCount, enqueueAll, cancellation);
        }

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(IEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
            int priority = 0, int attemptsCount = 1, bool enqueueAll = false, CancellationToken cancellation = default)
            where TResource : notnull
        {
            var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
            return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
                ? priorityAccessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                    priority,
                    attemptsCount,
                    enqueueAll,
                    cancellation)
                : accessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                    attemptsCount,
                    enqueueAll,
                    cancellation);
        }

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(IAsyncEnumerable<IAccess<TResource, TResult>> accesses, int priority = 0,
            int attemptsCount = 1, CancellationToken cancellation = default)
            where TResource : notnull
        {
            var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
            return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
                ? priorityAccessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)),
                    priority,
                    attemptsCount,
                    cancellation)
                : accessQueue.AccessAsync(accesses ?? throw new ArgumentNullException(nameof(accesses)), attemptsCount, cancellation);
        }

        /// <inheritdoc cref="AccessAsync{TResource,TResult}(IPriorityAccessQueue{TResource},IEnumerable{IAsyncAccess{TResource,TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> AccessAsync<TResource, TResult>(IAsyncEnumerable<IAsyncAccess<TResource, TResult>> asyncAccesses,
            int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
            where TResource : notnull
        {
            var accessQueue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IAccessQueue<TResource>>();
            return accessQueue is IPriorityAccessQueue<TResource> priorityAccessQueue
                ? priorityAccessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)),
                    priority,
                    attemptsCount,
                    cancellation)
                : accessQueue.AccessAsync(asyncAccesses ?? throw new ArgumentNullException(nameof(asyncAccesses)), attemptsCount, cancellation);
        }
    }
}
