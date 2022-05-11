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

using AInq.Background.Helpers;

namespace AInq.Background.Wrappers;

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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Maybe<Exception>>) CreateRepeatedWorkWrapper(IWork work, DateTime startTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount is > 0 or -1
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
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Try<TResult>>) CreateRepeatedWorkWrapper<TResult>(IWork<TResult> work, DateTime startTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="asyncWork" /> scheduled to <paramref name="startTime" /> </summary>
    /// <param name="asyncWork"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="asyncWork" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Maybe<Exception>>) CreateRepeatedWorkWrapper(IAsyncWork asyncWork, DateTime startTime,
        TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedTaskWrapper(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="asyncWork" /> scheduled to <paramref name="startTime" /> </summary>
    /// <param name="asyncWork"> Work instance </param>
    /// <param name="startTime"> Work first execution time </param>
    /// <param name="repeatDelay"> Work repeat delay </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="asyncWork" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="repeatDelay" /> isn't greater then 00:00:00.000 or <paramref name="execCount" /> is 0 or less then -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Try<TResult>>) CreateRepeatedWorkWrapper<TResult>(IAsyncWork<TResult> asyncWork,
        DateTime startTime, TimeSpan repeatDelay, CancellationToken cancellation = default, int execCount = -1)
    {
        startTime = startTime.ToLocalTime();
        var wrapper = new RepeatedTaskWrapper<TResult>(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)),
            startTime,
            repeatDelay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater then 00:00:00.000")
                : repeatDelay,
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"));
        return (wrapper, wrapper.WorkObservable);
    }

    private class RepeatedTaskWrapper : IScheduledTaskWrapper
    {
        private readonly IAsyncWork? _asyncWork;
        private readonly CancellationToken _innerCancellation;
        private readonly TimeSpan _repeatDelay;
        private readonly Subject<Maybe<Exception>> _subject = new();
        private readonly IWork? _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;
        private DateTime _nextScheduledTime;

        internal RepeatedTaskWrapper(IAsyncWork asyncWork, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation,
            int execCount)
        {
            _asyncWork = asyncWork;
            _work = null;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _subject.OnCompleted(), false);
        }

        internal RepeatedTaskWrapper(IWork work, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation, int execCount)
        {
            _asyncWork = null;
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _subject.OnCompleted(), false);
        }

        internal IObservable<Maybe<Exception>> WorkObservable => _subject;

        DateTime? IScheduledTaskWrapper.NextScheduledTime
            => _innerCancellation.IsCancellationRequested || _execCount == 0 ? null : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                if (_asyncWork == null)
                    _work!.DoWork(provider);
                else await _asyncWork.DoWorkAsync(provider, aggregateCancellation.Token).ConfigureAwait(false);
                _subject.OnNext(Maybe.None<Exception>());
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger.LogWarning("Scheduled work {Work} canceled by runtime", _asyncWork as object ?? _work);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing scheduled work {Work}", _asyncWork as object ?? _work);
                _subject.OnNext(Maybe.Value(ex));
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            if (_execCount != 0)
                return true;
            _subject.OnCompleted();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }
    }

    private class RepeatedTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly IAsyncWork<TResult>? _asyncWork;
        private readonly CancellationToken _innerCancellation;
        private readonly TimeSpan _repeatDelay;
        private readonly Subject<Try<TResult>> _subject = new();
        private readonly IWork<TResult>? _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;
        private DateTime _nextScheduledTime;

        internal RepeatedTaskWrapper(IAsyncWork<TResult> asyncWork, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation,
            int execCount)
        {
            _asyncWork = asyncWork;
            _work = null;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _subject.OnCompleted(), false);
        }

        internal RepeatedTaskWrapper(IWork<TResult> work, DateTime startTime, TimeSpan repeatDelay, CancellationToken innerCancellation,
            int execCount)
        {
            _asyncWork = null;
            _work = work;
            _innerCancellation = innerCancellation;
            _nextScheduledTime = startTime.ToLocalTime();
            _repeatDelay = repeatDelay;
            while (_nextScheduledTime < DateTime.Now) _nextScheduledTime += _repeatDelay;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _subject.OnCompleted(), false);
        }

        internal IObservable<Try<TResult>> WorkObservable => _subject;

        DateTime? IScheduledTaskWrapper.NextScheduledTime
            => _innerCancellation.IsCancellationRequested || _execCount == 0 ? null : _nextScheduledTime;

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                var result = _asyncWork == null
                    ? _work!.DoWork(provider)
                    : await _asyncWork.DoWorkAsync(provider, aggregateCancellation.Token).ConfigureAwait(false);
                _subject.OnNext(Try.Value(result));
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger.LogWarning("Scheduled work {Work} canceled by runtime", _asyncWork as object ?? _work);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing scheduled work {Work}", _asyncWork as object ?? _work);
                _subject.OnNext(Try.Error<TResult>(ex));
                _nextScheduledTime += _repeatDelay;
                if (_execCount != -1) _execCount--;
            }
            if (_execCount != 0)
                return true;
            _subject.OnCompleted();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }
    }
}
