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

/// <summary> <see cref="IWorkQueue" /> and <see cref="IPriorityWorkQueue" /> batch processing extension </summary>
public static class WorkQueueEnumerableExtension
{
    private static async void PushWorks<TResult>(ChannelWriter<Task<TResult>> writer, IWorkQueue queue, IAsyncEnumerable<IWork<TResult>> works,
        int attemptsCount, CancellationToken cancellation)
    {
        try
        {
            await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(queue.EnqueueWork(work, attemptsCount, cancellation), cancellation).ConfigureAwait(false);
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

    private static async void PushWorks<TResult>(ChannelWriter<Task<TResult>> writer, IPriorityWorkQueue priorityQueue,
        IAsyncEnumerable<IWork<TResult>> works, int priority, int attemptsCount, CancellationToken cancellation)
    {
        try
        {
            await foreach (var work in works.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(priorityQueue.EnqueueWork(work, priority, attemptsCount, cancellation), cancellation).ConfigureAwait(false);
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

    private static async void PushAsyncWorks<TResult>(ChannelWriter<Task<TResult>> writer, IWorkQueue queue,
        IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int attemptsCount, CancellationToken cancellation)
    {
        try
        {
            await foreach (var work in asyncWorks.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(queue.EnqueueAsyncWork(work, attemptsCount, cancellation), cancellation).ConfigureAwait(false);
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

    private static async void PushAsyncWorks<TResult>(ChannelWriter<Task<TResult>> writer, IPriorityWorkQueue priorityQueue,
        IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int priority, int attemptsCount, CancellationToken cancellation)
    {
        try
        {
            await foreach (var work in asyncWorks.WithCancellation(cancellation).ConfigureAwait(false))
                await writer.WriteAsync(priorityQueue.EnqueueAsyncWork(work, priority, attemptsCount, cancellation), cancellation)
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

    /// <param name="queue"> Work Queue instance </param>
    extension(IWorkQueue queue)
    {
        /// <summary> Batch process works </summary>
        /// <param name="works"> Works to be processed </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IEnumerable<IWork<TResult>> works, int attemptsCount = 1, bool enqueueAll = false,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = queue ?? throw new ArgumentNullException(nameof(queue));
            var results = (works ?? throw new ArgumentNullException(nameof(works))).Select(work
                => queue.EnqueueWork(work, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <summary> Batch process asynchronous works </summary>
        /// <param name="asyncWorks"> Works to be processed </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IEnumerable<IAsyncWork<TResult>> asyncWorks, int attemptsCount = 1,
            bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = queue ?? throw new ArgumentNullException(nameof(queue));
            var results = (asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks))).Select(work
                => queue.EnqueueAsyncWork(work, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IWork{TResult}},int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IAsyncEnumerable<IWork<TResult>> works, int attemptsCount = 1,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = queue ?? throw new ArgumentNullException(nameof(queue));
            _ = works ?? throw new ArgumentNullException(nameof(works));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushWorks(channel.Writer, queue, works, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAsyncWork{TResult}},int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int attemptsCount = 1,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = queue ?? throw new ArgumentNullException(nameof(queue));
            _ = asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushAsyncWorks(channel.Writer, queue, asyncWorks, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }

    /// <param name="priorityQueue"> Work Queue instance </param>
    extension(IPriorityWorkQueue priorityQueue)
    {
        /// <summary> Batch process works with giver <paramref name="priority" /> </summary>
        /// <param name="works"> Works to be processed </param>
        /// <param name="priority"> Operation priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IEnumerable<IWork<TResult>> works, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
            var results = (works ?? throw new ArgumentNullException(nameof(works))).Select(work
                => priorityQueue.EnqueueWork(work, priority, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <summary> Batch process works asynchronous with giver <paramref name="priority" /> </summary>
        /// <param name="asyncWorks"> Works to be processed </param>
        /// <param name="priority"> Operation priority </param>
        /// <param name="attemptsCount"> Retry on fail attempts count </param>
        /// <param name="enqueueAll"> Option to enqueue all data first </param>
        /// <param name="cancellation"> Processing cancellation token </param>
        /// <typeparam name="TResult"> Processing result type </typeparam>
        /// <returns> Processing result task enumeration </returns>
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IEnumerable<IAsyncWork<TResult>> asyncWorks, int priority = 0,
            int attemptsCount = 1, bool enqueueAll = false, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
            var results = (asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks))).Select(work
                => priorityQueue.EnqueueAsyncWork(work, priority, attemptsCount, cancellation));
            if (enqueueAll) results = results.ToList();
            foreach (var result in results)
                yield return await result.ConfigureAwait(false);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IPriorityWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IWork{TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IAsyncEnumerable<IWork<TResult>> works, int priority = 0, int attemptsCount = 1,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
            _ = works ?? throw new ArgumentNullException(nameof(works));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushWorks(channel.Writer, priorityQueue, works, priority, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IPriorityWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAsyncWork{TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public async IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int priority = 0,
            int attemptsCount = 1, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
            _ = asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks));
            var channel = Channel.CreateUnbounded<Task<TResult>>(new UnboundedChannelOptions {SingleReader = true, SingleWriter = true});
            var reader = channel.Reader;
            PushAsyncWorks(channel.Writer, priorityQueue, asyncWorks, priority, attemptsCount, cancellation);
            while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false))
                yield return await (await reader.ReadAsync(cancellation).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }

    /// <param name="works"> Works to be processed </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResult>(IEnumerable<IWork<TResult>> works)
    {
        /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IWorkQueue queue, int attemptsCount = 1, bool enqueueAll = false,
            CancellationToken cancellation = default)
            => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)),
                attemptsCount,
                enqueueAll,
                cancellation);

        /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IPriorityWorkQueue priorityQueue, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, CancellationToken cancellation = default)
            => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue))).DoWorkAsync(
                works ?? throw new ArgumentNullException(nameof(works)),
                priority,
                attemptsCount,
                enqueueAll,
                cancellation);
    }

    /// <param name="asyncWorks"> Works to be processed </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResult>(IEnumerable<IAsyncWork<TResult>> asyncWorks)
    {
        /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IWorkQueue queue, int attemptsCount = 1, bool enqueueAll = false,
            CancellationToken cancellation = default)
            => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(
                asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)),
                attemptsCount,
                enqueueAll,
                cancellation);

        /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IPriorityWorkQueue priorityQueue, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, CancellationToken cancellation = default)
            => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue))).DoWorkAsync(
                asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)),
                priority,
                attemptsCount,
                enqueueAll,
                cancellation);
    }

    /// <param name="works"> Works to be processed </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResult>(IAsyncEnumerable<IWork<TResult>> works)
    {
        /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IWork{TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IWorkQueue queue, int attemptsCount = 1, CancellationToken cancellation = default)
            => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)),
                attemptsCount,
                cancellation);

        /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IPriorityWorkQueue priorityQueue, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue))).DoWorkAsync(
                works ?? throw new ArgumentNullException(nameof(works)),
                priority,
                attemptsCount,
                cancellation);
    }

    /// <param name="asyncWorks"> Works to be processed </param>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    extension<TResult>(IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks)
    {
        /// <inheritdoc cref="DoWorkAsync{TResult}(IWorkQueue,IEnumerable{IAsyncWork{TResult}},int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IWorkQueue queue, int attemptsCount = 1, CancellationToken cancellation = default)
            => (queue ?? throw new ArgumentNullException(nameof(queue))).DoWorkAsync(
                asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)),
                attemptsCount,
                cancellation);

        /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IAsyncWork{TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync(IPriorityWorkQueue priorityQueue, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue))).DoWorkAsync(
                asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)),
                priority,
                attemptsCount,
                cancellation);
    }

    /// <param name="provider"> Service provider instance </param>
    extension(IServiceProvider provider)
    {
        /// <inheritdoc cref="DoWorkAsync{TResult}(IPriorityWorkQueue,IEnumerable{IWork{TResult}},int,int,bool,CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IEnumerable<IWork<TResult>> works, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, CancellationToken cancellation = default)
        {
            var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
            return queue is IPriorityWorkQueue priorityQueue
                ? priorityQueue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)),
                    priority,
                    attemptsCount,
                    enqueueAll,
                    cancellation)
                : queue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), attemptsCount, enqueueAll, cancellation);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IPriorityWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAsyncWork{TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IEnumerable<IAsyncWork<TResult>> asyncWorks, int priority = 0, int attemptsCount = 1,
            bool enqueueAll = false, CancellationToken cancellation = default)
        {
            var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
            return queue is IPriorityWorkQueue priorityQueue
                ? priorityQueue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)),
                    priority,
                    attemptsCount,
                    enqueueAll,
                    cancellation)
                : queue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), attemptsCount, enqueueAll, cancellation);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IPriorityWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IWork{TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IAsyncEnumerable<IWork<TResult>> works, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
        {
            var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
            return queue is IPriorityWorkQueue priorityQueue
                ? priorityQueue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), priority, attemptsCount, cancellation)
                : queue.DoWorkAsync(works ?? throw new ArgumentNullException(nameof(works)), attemptsCount, cancellation);
        }

        /// <inheritdoc cref="WorkQueueEnumerableExtension.DoWorkAsync{TResult}(AInq.Background.Services.IPriorityWorkQueue,System.Collections.Generic.IEnumerable{AInq.Background.Tasks.IAsyncWork{TResult}},int,int,bool,System.Threading.CancellationToken)" />
        [PublicAPI]
        public IAsyncEnumerable<TResult> DoWorkAsync<TResult>(IAsyncEnumerable<IAsyncWork<TResult>> asyncWorks, int priority = 0,
            int attemptsCount = 1, CancellationToken cancellation = default)
        {
            var queue = (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkQueue>();
            return queue is IPriorityWorkQueue priorityQueue
                ? priorityQueue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), priority, attemptsCount, cancellation)
                : queue.DoWorkAsync(asyncWorks ?? throw new ArgumentNullException(nameof(asyncWorks)), attemptsCount, cancellation);
        }
    }
}
