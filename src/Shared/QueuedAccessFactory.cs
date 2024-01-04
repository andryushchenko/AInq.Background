// Copyright 2020-2024 Anton Andryushchenko
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

internal static class QueuedAccessFactory
{
    public static IAsyncWork CreateQueuedAccess<TResource>(IAccess<TResource> access, int priority, int attemptsCount)
        where TResource : notnull
        => new QueuedAccess<TResource>(access ?? throw new ArgumentNullException(nameof(access)), priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedAccess<TResource, TResult>(IAccess<TResource, TResult> access, int priority, int attemptsCount)
        where TResource : notnull
        => new QueuedAccess<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)), priority, attemptsCount);

    public static IAsyncWork CreateQueuedAsyncAccess<TResource>(IAsyncAccess<TResource> asyncAccess, int priority, int attemptsCount)
        where TResource : notnull
        => new QueuedAsyncAccess<TResource>(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedAsyncAccess<TResource, TResult>(IAsyncAccess<TResource, TResult> asyncAccess, int priority,
        int attemptsCount)
        where TResource : notnull
        => new QueuedAsyncAccess<TResource, TResult>(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)), priority, attemptsCount);

    public static IAsyncWork CreateQueuedInjectedAccess<TResource, TAccess>(int priority, int attemptsCount)
        where TResource : notnull
        where TAccess : IAccess<TResource>
        => new QueuedInjectedAccess<TResource, TAccess>(priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedInjectedAccess<TResource, TAccess, TResult>(int priority, int attemptsCount)
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
        => new QueuedInjectedAccess<TResource, TAccess, TResult>(priority, attemptsCount);

    public static IAsyncWork CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess>(int priority, int attemptsCount)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
        => new QueuedInjectedAsyncAccess<TResource, TAsyncAccess>(priority, attemptsCount);

    public static IAsyncWork<TResult> CreateQueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(int priority, int attemptsCount)
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
        => new QueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult>(priority, attemptsCount);

    private class QueuedAccess<TResource> : IAsyncWork
        where TResource : notnull
    {
        private readonly IAccess<TResource> _access;
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedAccess(IAccess<TResource> access, int priority, int attemptsCount)
        {
            _access = access ?? throw new ArgumentNullException(nameof(access));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAccess(_access, _priority, _attemptsCount, cancellation);
    }

    private class QueuedAccess<TResource, TResult> : IAsyncWork<TResult>
        where TResource : notnull
    {
        private readonly IAccess<TResource, TResult> _access;
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedAccess(IAccess<TResource, TResult> access, int priority, int attemptsCount)
        {
            _access = access ?? throw new ArgumentNullException(nameof(access));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAccess(_access, _priority, _attemptsCount, cancellation);
    }

    private class QueuedAsyncAccess<TResource> : IAsyncWork
        where TResource : notnull
    {
        private readonly IAsyncAccess<TResource> _asyncAccess;
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedAsyncAccess(IAsyncAccess<TResource> asyncAccess, int priority, int attemptsCount)
        {
            _asyncAccess = asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncAccess(_asyncAccess, _priority, _attemptsCount, cancellation);
    }

    private class QueuedAsyncAccess<TResource, TResult> : IAsyncWork<TResult>
        where TResource : notnull
    {
        private readonly IAsyncAccess<TResource, TResult> _asyncAccess;
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedAsyncAccess(IAsyncAccess<TResource, TResult> asyncAccess, int priority, int attemptsCount)
        {
            _asyncAccess = asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess));
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncAccess(_asyncAccess, _priority, _attemptsCount, cancellation);
    }

    private class QueuedInjectedAccess<TResource, TAccess> : IAsyncWork
        where TResource : notnull
        where TAccess : IAccess<TResource>
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedAccess(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAccess<TResource, TAccess>(_priority, _attemptsCount, cancellation);
    }

    private class QueuedInjectedAccess<TResource, TAccess, TResult> : IAsyncWork<TResult>
        where TResource : notnull
        where TAccess : IAccess<TResource, TResult>
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedAccess(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAccess<TResource, TAccess, TResult>(_priority, _attemptsCount, cancellation);
    }

    private class QueuedInjectedAsyncAccess<TResource, TAsyncAccess> : IAsyncWork
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource>
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedAsyncAccess(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncAccess<TResource, TAsyncAccess>(_priority, _attemptsCount, cancellation);
    }

    private class QueuedInjectedAsyncAccess<TResource, TAsyncAccess, TResult> : IAsyncWork<TResult>
        where TResource : notnull
        where TAsyncAccess : IAsyncAccess<TResource, TResult>
    {
        private readonly int _attemptsCount;
        private readonly int _priority;

        public QueuedInjectedAsyncAccess(int priority, int attemptsCount)
        {
            _attemptsCount = attemptsCount;
            _priority = priority;
        }

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => serviceProvider.EnqueueAsyncAccess<TResource, TAsyncAccess, TResult>(_priority, _attemptsCount, cancellation);
    }
}
