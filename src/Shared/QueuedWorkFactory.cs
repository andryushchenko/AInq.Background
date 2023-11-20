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

    private class QueuedWork : IAsyncWork

    {
        private readonly int _attemptsCount;
        private readonly int _priority;
        private readonly IWork _work;

        public QueuedWork(IWork work, int priority, int attemptsCount)
        {
            _work = work ?? throw new ArgumentNullException(nameof(work));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork(_work, cancellation, _attemptsCount, _priority);
    }

    private class QueuedWork<TResult> : IAsyncWork<TResult>

    {
        private readonly int _attemptsCount;
        private readonly int _priority;
        private readonly IWork<TResult> _work;

        public QueuedWork(IWork<TResult> work, int priority, int attemptsCount)
        {
            _work = work ?? throw new ArgumentNullException(nameof(work));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork(_work, cancellation, _attemptsCount, _priority);
    }

    private class QueuedAsyncWork : IAsyncWork

    {
        private readonly IAsyncWork _asyncWork;
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedAsyncWork(IAsyncWork asyncWork, int priority, int attemptsCount)
        {
            _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork(_asyncWork, cancellation, _attemptsCount, _priority);
    }

    private class QueuedAsyncWork<TResult> : IAsyncWork<TResult>

    {
        private readonly IAsyncWork<TResult> _asyncWork;
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedAsyncWork(IAsyncWork<TResult> asyncWork, int priority, int attemptsCount)
        {
            _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork(_asyncWork, cancellation, _attemptsCount, _priority);
    }

    private class QueuedInjectedWork<TWork> : IAsyncWork
        where TWork : IWork
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedWork(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork<TWork>(cancellation, _attemptsCount, _priority);
    }

    private class QueuedInjectedWork<TWork, TResult> : IAsyncWork<TResult>
        where TWork : IWork<TResult>
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedWork(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueWork<TWork, TResult>(cancellation, _attemptsCount, _priority);
    }

    private class QueuedInjectedAsyncWork<TAsyncWork> : IAsyncWork
        where TAsyncWork : IAsyncWork
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedAsyncWork(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork<TAsyncWork>(cancellation, _attemptsCount, _priority);
    }

    private class QueuedInjectedAsyncWork<TAsyncWork, TResult> : IAsyncWork<TResult>
        where TAsyncWork : IAsyncWork<TResult>
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedAsyncWork(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, _attemptsCount, _priority);
    }
}
