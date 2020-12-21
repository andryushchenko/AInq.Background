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

/// <summary> Factory class for creating <see cref="IScheduledTaskWrapper" /> for repeated work </summary>
public static class RepeatedWorkWrapperFactory
{
    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="startTime" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<object?>) CreateRepeatedWorkWrapper(IWork work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount > 0 || execCount == -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="startTime" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<TResult>) CreateRepeatedWorkWrapper<TResult>(IWork<TResult> work, DateTime startTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount > 0 || execCount == -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="startTime" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<object?>) CreateRepeatedWorkWrapper(IAsyncWork work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedAsyncTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount > 0 || execCount == -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled to <paramref name="startTime" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or
    ///     <paramref name="execCount" /> is 0 or less then -1
    /// </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<TResult>) CreateRepeatedWorkWrapper<TResult>(IAsyncWork<TResult> work, DateTime startTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedAsyncTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount > 0 || execCount == -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    private class RepeatedTaskWrapper : IScheduledTaskWrapper
    {
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<object?> _observable = new();
        private readonly TimeSpan _repeatDelay;
        private readonly IWork _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;
        private DateTime _nextScheduledTime;

        internal RepeatedTaskWrapper(IWork work, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation, int execCount)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<object?> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime
            => _innerCancellation.IsCancellationRequested || _execCount == 0 ? null : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _work.DoWork(provider);
                _observable.Next(null);
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
                _observable.Error(ex);
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            if (_execCount != 0)
                return Task.FromResult(true);
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(false);
        }
    }

    private class RepeatedTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<TResult> _observable = new();
        private readonly TimeSpan _repeatDelay;
        private readonly IWork<TResult> _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;
        private DateTime _nextScheduledTime;

        internal RepeatedTaskWrapper(IWork<TResult> work, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation,
            int execCount)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<TResult> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime
            => _innerCancellation.IsCancellationRequested || _execCount == 0 ? null : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _observable.Next(_work.DoWork(provider));
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
                _observable.Error(ex);
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            if (_execCount != 0)
                return Task.FromResult(true);
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(false);
        }
    }

    private class RepeatedAsyncTaskWrapper : IScheduledTaskWrapper
    {
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<object?> _observable = new();
        private readonly TimeSpan _repeatDelay;
        private readonly IAsyncWork _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;
        private DateTime _nextScheduledTime;

        internal RepeatedAsyncTaskWrapper(IAsyncWork work, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation,
            int execCount)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<object?> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime
            => _innerCancellation.IsCancellationRequested || _execCount == 0 ? null : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                await _work.DoWorkAsync(provider, aggregateCancellation.Token);
                _observable.Next(null);
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
                _observable.Error(ex);
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            if (_execCount != 0)
                return true;
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }
    }

    private class RepeatedAsyncTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<TResult> _observable = new();
        private readonly TimeSpan _repeatDelay;
        private readonly IAsyncWork<TResult> _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;
        private DateTime _nextScheduledTime;

        internal RepeatedAsyncTaskWrapper(IAsyncWork<TResult> work, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation,
            int execCount)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<TResult> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime
            => _innerCancellation.IsCancellationRequested || _execCount == 0 ? null : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _observable.Next(await _work.DoWorkAsync(provider, aggregateCancellation.Token));
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
                _observable.Error(ex);
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            if (_execCount != 0)
                return true;
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }
    }
}

}
