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

using AInq.Background.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

/// <summary> Factory class for creating <see cref="IScheduledTaskWrapper" /> for once scheduled work </summary>
public static class DelayedWorkWrapperFactory
{
    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="time" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Scheduled time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> is less or equal to current time </exception>
    /// <returns> Wrapper and work result task </returns>
    public static (IScheduledTaskWrapper, Task) CreateDelayedWorkWrapper(IWork work, DateTime time, CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        var wrapper = new DelayedTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="time" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Scheduled time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> is less or equal to current time </exception>
    /// <returns> Wrapper and work result task </returns>
    public static (IScheduledTaskWrapper, Task<TResult>) CreateDelayedWorkWrapper<TResult>(IWork<TResult> work, DateTime time,
        CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        var wrapper = new DelayedTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="time" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Scheduled time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> is less or equal to current time </exception>
    /// <returns> Wrapper and work result task </returns>
    public static (IScheduledTaskWrapper, Task) CreateDelayedWorkWrapper(IAsyncWork work, DateTime time, CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        var wrapper = new DelayedAsyncTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="time" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="time"> Scheduled time </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="time" /> is less or equal to current time </exception>
    /// <returns> Wrapper and work result task </returns>
    public static (IScheduledTaskWrapper, Task<TResult>) CreateDelayedWorkWrapper<TResult>(IAsyncWork<TResult> work, DateTime time,
        CancellationToken cancellation = default)
    {
        time = time.ToLocalTime();
        var wrapper = new DelayedAsyncTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    private class DelayedTaskWrapper : IScheduledTaskWrapper
    {
        private readonly TaskCompletionSource<bool> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private readonly IWork _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private DateTime? _nextScheduledTime;

        internal DelayedTaskWrapper(IWork work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task WorkTask => _completion.Task;
        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _work.DoWork(provider);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled work {Work} canceled by runtime", _work);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled work {Work}", _work);
                _completion.TrySetException(ex);
            }
            _nextScheduledTime = null;
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(false);
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested ? null : _nextScheduledTime;
    }

    private class DelayedTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly TaskCompletionSource<TResult> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private readonly IWork<TResult> _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private DateTime? _nextScheduledTime;

        internal DelayedTaskWrapper(IWork<TResult> work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> WorkTask => _completion.Task;
        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _completion.TrySetResult(_work.DoWork(provider));
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled work {Work} canceled by runtime", _work);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled work {Work}", _work);
                _completion.TrySetException(ex);
            }
            _nextScheduledTime = null;
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(false);
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested ? null : _nextScheduledTime;
    }

    private class DelayedAsyncTaskWrapper : IScheduledTaskWrapper
    {
        private readonly TaskCompletionSource<bool> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private readonly IAsyncWork _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private DateTime? _nextScheduledTime;

        internal DelayedAsyncTaskWrapper(IAsyncWork work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task WorkTask => _completion.Task;
        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                await _work.DoWorkAsync(provider, aggregateCancellation.Token);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled work {Work} canceled by runtime", _work);
                return true;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled work {Work}", _work);
                _completion.TrySetException(ex);
            }
            _nextScheduledTime = null;
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested ? null : _nextScheduledTime;
    }

    private class DelayedAsyncTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly TaskCompletionSource<TResult> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private readonly IAsyncWork<TResult> _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private DateTime? _nextScheduledTime;

        internal DelayedAsyncTaskWrapper(IAsyncWork<TResult> work, DateTime time, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = time;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> WorkTask => _completion.Task;
        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(await _work.DoWorkAsync(provider, aggregateCancellation.Token));
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled work {Work} canceled by runtime", _work);
                return true;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled work {Work}", _work);
                _completion.TrySetException(ex);
            }
            _nextScheduledTime = null;
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested ? null : _nextScheduledTime;
    }
}

}
