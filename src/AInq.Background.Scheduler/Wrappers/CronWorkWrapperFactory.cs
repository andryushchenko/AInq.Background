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
using Cronos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

/// <summary> Factory class for creating <see cref="IScheduledTaskWrapper" /> for CRON scheduled work </summary>
public static class CronWorkWrapperFactory
{
    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cron" /> is NULL </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<object?>) CreateCronWorkWrapper(IWork work, CronExpression cron,
        CancellationToken cancellation = default, int execCount = -1)
    {
        var wrapper = new CronTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation,
            execCount);
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cron" /> is NULL </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<TResult>) CreateCronWorkWrapper<TResult>(IWork<TResult> work, CronExpression cron,
        CancellationToken cancellation = default, int execCount = -1)
    {
        var wrapper = new CronTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation,
            execCount);
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cron" /> is NULL </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<object?>) CreateCronWorkWrapper(IAsyncWork work, CronExpression cron,
        CancellationToken cancellation = default, int execCount = -1)
    {
        var wrapper = new CronAsyncTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation,
            execCount);
        return (wrapper, wrapper.WorkObservable);
    }

    /// <summary> Create <see cref="IScheduledTaskWrapper" /> for given <paramref name="work" /> scheduled by <paramref name="cron" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cron"> Cron expression </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <param name="execCount"> Max work execution count (-1 for unlimited) </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> or <paramref name="cron" /> is NULL </exception>
    /// <returns> Wrapper and work result observable </returns>
    public static (IScheduledTaskWrapper, IObservable<TResult>) CreateCronWorkWrapper<TResult>(IAsyncWork<TResult> work, CronExpression cron,
        CancellationToken cancellation = default, int execCount = -1)
    {
        var wrapper = new CronAsyncTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation,
            execCount);
        return (wrapper, wrapper.WorkObservable);
    }

    private class CronTaskWrapper : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<object?> _observable = new();
        private readonly IWork _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;

        internal CronTaskWrapper(IWork work, CronExpression cron, CancellationToken innerCancellation, int execCount)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<object?> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)?.ToLocalTime();

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _work.DoWork(provider);
                _observable.Next(null);
                if (_execCount < 0) _execCount--;
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
                if (_execCount < 0) _execCount--;
            }
            if (_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue && _execCount != 0)
                return Task.FromResult(true);
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(false);
        }
    }

    private class CronTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<TResult> _observable = new();
        private readonly IWork<TResult> _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;

        internal CronTaskWrapper(IWork<TResult> work, CronExpression cron, CancellationToken innerCancellation, int execCount)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<TResult> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)?.ToLocalTime();

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _observable.Next(_work.DoWork(provider));
                if (_execCount < 0) _execCount--;
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
                if (_execCount < 0) _execCount--;
            }
            if (_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue && _execCount != 0)
                return Task.FromResult(true);
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(false);
        }
    }

    private class CronAsyncTaskWrapper : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<object?> _observable = new();
        private readonly IAsyncWork _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;

        internal CronAsyncTaskWrapper(IAsyncWork work, CronExpression cron, CancellationToken innerCancellation, int execCount)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<object?> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)?.ToLocalTime();

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                await _work.DoWorkAsync(provider, aggregateCancellation.Token);
                _observable.Next(null);
                if (_execCount < 0) _execCount--;
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
                if (_execCount < 0) _execCount--;
            }
            if (_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue && _execCount != 0)
                return true;
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }
    }

    private class CronAsyncTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly Observable<TResult> _observable = new();
        private readonly IAsyncWork<TResult> _work;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _execCount;

        internal CronAsyncTaskWrapper(IAsyncWork<TResult> work, CronExpression cron, CancellationToken innerCancellation, int execCount)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
            _execCount = execCount;
            _cancellationRegistration = _innerCancellation.Register(() => _observable.Complete(), false);
        }

        internal IObservable<TResult> WorkObservable => _observable;

        DateTime? IScheduledTaskWrapper.NextScheduledTime => _innerCancellation.IsCancellationRequested
            ? null
            : _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)?.ToLocalTime();

        bool IScheduledTaskWrapper.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> IScheduledTaskWrapper.ExecuteAsync(IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _observable.Next(await _work.DoWorkAsync(provider, aggregateCancellation.Token));
                if (_execCount < 0) _execCount--;
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
                if (_execCount < 0) _execCount--;
            }
            if (_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue && _execCount != 0)
                return true;
            _observable.Complete();
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return false;
        }
    }
}

}
