/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

internal static class DelayedWorkWrapperFactory
{
    private class DelayedTaskWrapper : IScheduledTaskWrapper
    {
        private readonly IWork _work;
        private readonly CancellationToken _innerCancellation;
        private DateTime? _nextScheduledTime;

        internal DelayedTaskWrapper(IWork work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _work.DoWork(provider);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            _nextScheduledTime = null;
            return Task.FromResult(false);
        }
    }

    private class DelayedTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly IWork<TResult> _work;
        private readonly CancellationToken _innerCancellation;
        private DateTime? _nextScheduledTime;

        internal DelayedTaskWrapper(IWork<TResult> work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _work.DoWork(provider);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            _nextScheduledTime = null;
            return Task.FromResult(false);
        }
    }

    private class DelayedAsyncTaskWrapper : IScheduledTaskWrapper
    {
        private readonly IAsyncWork _work;
        private readonly CancellationToken _innerCancellation;
        private DateTime? _nextScheduledTime;

        internal DelayedAsyncTaskWrapper(IAsyncWork work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                await _work.DoWorkAsync(provider, aggregateCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
                return true;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            _nextScheduledTime = null;
            return false;
        }
    }

    private class DelayedAsyncTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly IAsyncWork<TResult> _work;
        private readonly CancellationToken _innerCancellation;
        private DateTime? _nextScheduledTime;

        internal DelayedAsyncTaskWrapper(IAsyncWork<TResult> work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                await _work.DoWorkAsync(provider, aggregateCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
                return true;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            _nextScheduledTime = null;
            return false;
        }
    }

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper(IWork work, DateTime time, CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        return new DelayedTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
    }

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper(IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => new DelayedTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            DateTime.Now.Add(delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay),
            cancellation);

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        return new DelayedTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
    }

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
        => new DelayedTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            DateTime.Now.Add(delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay),
            cancellation);

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper(IAsyncWork work, DateTime time, CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        return new DelayedAsyncTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
    }

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper(IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
        => new DelayedAsyncTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            DateTime.Now.Add(delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay),
            cancellation);

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        return new DelayedAsyncTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
    }

    public static IScheduledTaskWrapper CreateDelayedWorkWrapper<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
        => new DelayedAsyncTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            DateTime.Now.Add(delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay),
            cancellation);
}

}
