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

using AInq.Background.Helpers;
using Cronos;

namespace AInq.Background.Wrappers;

/// <summary> Factory class for creating <see cref="IScheduledTaskWrapper" /> for CRON scheduled work </summary>
public static class CronWorkWrapperFactory
{
    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Maybe<Exception>>) CreateCronWorkWrapper(IWork work, CronExpression cron, int execCount = -1,
        CancellationToken cancellation = default)
    {
        var wrapper = new CronTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"),
            cancellation);
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Try<TResult>>) CreateCronWorkWrapper<TResult>(IWork<TResult> work, CronExpression cron,
        int execCount = -1, CancellationToken cancellation = default)
    {
        var wrapper = new CronTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"),
            cancellation);
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Maybe<Exception>>) CreateCronWorkWrapper(IAsyncWork work, CronExpression cron,
        int execCount = -1, CancellationToken cancellation = default)
    {
        var wrapper = new CronTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"),
            cancellation);
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="execCount" /> is 0 or less than -1 </exception>
    /// <returns> Wrapper and work result observable </returns>
    [PublicAPI]
    public static (IScheduledTaskWrapper, IObservable<Try<TResult>>) CreateCronWorkWrapper<TResult>(IAsyncWork<TResult> work, CronExpression cron,
        int execCount = -1, CancellationToken cancellation = default)
    {
        var wrapper = new CronTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater then 0 or -1 for unlimited repeat"),
            cancellation);
        return (wrapper, wrapper.WorkObservable);
    }

    private class CronTaskWrapper : IScheduledTaskWrapper
    {
        private readonly IAsyncWork? _asyncWork;
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly Subject<Maybe<Exception>> _subject = new();
        private readonly IWork? _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;

        internal CronTaskWrapper(IAsyncWork asyncWork, CronExpression cron, int execCount, CancellationToken innerCancellation)
        {
            _asyncWork = asyncWork;
            _work = null;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(_subject.OnCompleted, false);
        }

        internal CronTaskWrapper(IWork work, CronExpression cron, int execCount, CancellationToken innerCancellation)
        {
            _asyncWork = null;
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(_subject.OnCompleted, false);
        }

        internal IObservable<Maybe<Exception>> WorkObservable => _subject;

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested || _execCount == 0
            ? null
            : _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)?.ToLocalTime();

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
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested && logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning("Scheduled work {Work} canceled by runtime", _asyncWork as object ?? _work);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Error processing scheduled work {Work}", _asyncWork as object ?? _work);
                _subject.OnNext(ex);
                if (_execCount != -1) _execCount--;
            }
            if (_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue && _execCount != 0)
                return true;
            _subject.OnCompleted();
            await _cancellationRegistration.DisposeAsync().ConfigureAwait(false);
            _cancellationRegistration = default;
            return false;
        }
    }

    private class CronTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly IAsyncWork<TResult>? _asyncWork;
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly Subject<Try<TResult>> _subject = new();
        private readonly IWork<TResult>? _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;

        internal CronTaskWrapper(IAsyncWork<TResult> asyncWork, CronExpression cron, int execCount, CancellationToken innerCancellation)
        {
            _asyncWork = asyncWork;
            _work = null;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(_subject.OnCompleted, false);
        }

        internal CronTaskWrapper(IWork<TResult> work, CronExpression cron, int execCount, CancellationToken innerCancellation)
        {
            _asyncWork = null;
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(_subject.OnCompleted, false);
        }

        internal IObservable<Try<TResult>> WorkObservable => _subject;

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested || _execCount == 0
            ? null
            : _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)?.ToLocalTime();

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
                _subject.OnNext(result);
                if (_execCount != -1) _execCount--;
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested && logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning("Scheduled work {Work} canceled by runtime", _asyncWork as object ?? _work);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Error processing scheduled work {Work}", _asyncWork as object ?? _work);
                _subject.OnNext(ex);
                if (_execCount != -1) _execCount--;
            }
            if (_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue && _execCount != 0)
                return true;
            _subject.OnCompleted();
            await _cancellationRegistration.DisposeAsync().ConfigureAwait(false);
            _cancellationRegistration = default;
            return false;
        }
    }
}
