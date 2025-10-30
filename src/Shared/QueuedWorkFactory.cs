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

namespace AInq.Background.Tasks;

internal static class QueuedWorkFactory
{
    public static IAsyncWork CreateQueuedWork(IWork work, int priority, int attemptsCount)
        => new QueuedWork(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedWork<TResult>(IWork<TResult> work, int priority, int attemptsCount)
        => new QueuedWork<TResult>(work ?? throw new ArgumentNullException(nameof(work)), priority, attemptsCount);

    public static IAsyncWork CreateQueuedAsyncWork(IAsyncWork asyncWork, int priority, int attemptsCount)
        => new QueuedAsyncWork(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedAsyncWork<TResult>(IAsyncWork<TResult> asyncWork, int priority, int attemptsCount)
        => new QueuedAsyncWork<TResult>(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), priority, attemptsCount);

    public static IAsyncWork CreateQueuedInjectedWork<TWork>(int priority, int attemptsCount)
        where TWork : IWork
        => new QueuedInjectedWork<TWork>(priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedInjectedWork<TWork, TResult>(int priority, int attemptsCount)
        where TWork : IWork<TResult>
        => new QueuedInjectedWork<TWork, TResult>(priority, attemptsCount);

    public static IAsyncWork CreateQueuedInjectedAsyncWork<TAsyncWork>(int priority, int attemptsCount)
        where TAsyncWork : IAsyncWork
        => new QueuedInjectedAsyncWork<TAsyncWork>(priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedInjectedAsyncWork<TAsyncWork, TResult>(int priority, int attemptsCount)
        where TAsyncWork : IAsyncWork<TResult>
        => new QueuedInjectedAsyncWork<TAsyncWork, TResult>(priority, attemptsCount);

    private class QueuedWork(IWork work, int priority, int attemptsCount) : IAsyncWork

    {
        private readonly IWork _work = work ?? throw new ArgumentNullException(nameof(work));

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork(_work, priority, attemptsCount, cancellation);
    }

    private class QueuedWork<TResult>(IWork<TResult> work, int priority, int attemptsCount) : IAsyncWork<TResult>

    {
        private readonly IWork<TResult> _work = work ?? throw new ArgumentNullException(nameof(work));

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork(_work, priority, attemptsCount, cancellation);
    }

    private class QueuedAsyncWork(IAsyncWork asyncWork, int priority, int attemptsCount) : IAsyncWork

    {
        private readonly IAsyncWork _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork(_asyncWork, priority, attemptsCount, cancellation);
    }

    private class QueuedAsyncWork<TResult>(IAsyncWork<TResult> asyncWork, int priority, int attemptsCount) : IAsyncWork<TResult>

    {
        private readonly IAsyncWork<TResult> _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork(_asyncWork, priority, attemptsCount, cancellation);
    }

    private class QueuedInjectedWork<TWork>(int priority, int attemptsCount) : IAsyncWork
        where TWork : IWork
    {
        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork<TWork>(priority, attemptsCount, cancellation);
    }

    private class QueuedInjectedWork<TWork, TResult>(int priority, int attemptsCount) : IAsyncWork<TResult>
        where TWork : IWork<TResult>
    {
        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork<TWork, TResult>(priority, attemptsCount, cancellation);
    }

    private class QueuedInjectedAsyncWork<TAsyncWork>(int priority, int attemptsCount) : IAsyncWork
        where TAsyncWork : IAsyncWork
    {
        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork<TAsyncWork>(priority, attemptsCount, cancellation);
    }

    private class QueuedInjectedAsyncWork<TAsyncWork, TResult>(int priority, int attemptsCount) : IAsyncWork<TResult>
        where TAsyncWork : IAsyncWork<TResult>
    {
        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork<TAsyncWork, TResult>(priority, attemptsCount, cancellation);
    }
}
