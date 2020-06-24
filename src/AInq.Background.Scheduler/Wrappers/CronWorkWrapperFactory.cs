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

internal static class CronWorkWrapperFactory
{
    public static IScheduledTaskWrapper CreateCronWorkWrapper(IWork work, CronExpression cron, CancellationToken cancellation = default)
        => new CronTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation);

    public static IScheduledTaskWrapper CreateCronWorkWrapper<TResult>(IWork<TResult> work, CronExpression cron,
        CancellationToken cancellation = default)
        => new CronTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation);

    public static IScheduledTaskWrapper CreateCronWorkWrapper(IAsyncWork work, CronExpression cron, CancellationToken cancellation = default)
        => new CronAsyncTaskWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation);

    public static IScheduledTaskWrapper CreateCronWorkWrapper<TResult>(IAsyncWork<TResult> work, CronExpression cron,
        CancellationToken cancellation = default)
        => new CronAsyncTaskWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)),
            cron ?? throw new ArgumentNullException(nameof(cron)),
            cancellation);

    internal static CronExpression? ParseCron(this string cronExpression)
    {
        try
        {
            return cronExpression.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Length switch
            {
                5 => CronExpression.Parse(cronExpression, CronFormat.Standard),
                6 => CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds),
                _ => null
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private class CronTaskWrapper : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly IWork _work;

        internal CronTaskWrapper(IWork work, CronExpression cron, CancellationToken innerCancellation)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
        }

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
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            return Task.FromResult(_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue);
        }
    }

    private class CronTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly IWork<TResult> _work;

        internal CronTaskWrapper(IWork<TResult> work, CronExpression cron, CancellationToken innerCancellation)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
        }

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
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            return Task.FromResult(_cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue);
        }
    }

    private class CronAsyncTaskWrapper : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly IAsyncWork _work;

        internal CronAsyncTaskWrapper(IAsyncWork work, CronExpression cron, CancellationToken innerCancellation)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
        }

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
                await _work.DoWorkAsync(provider, aggregateCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            return _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue;
        }
    }

    private class CronAsyncTaskWrapper<TResult> : IScheduledTaskWrapper
    {
        private readonly CronExpression _cron;
        private readonly CancellationToken _innerCancellation;
        private readonly IAsyncWork<TResult> _work;

        internal CronAsyncTaskWrapper(IAsyncWork<TResult> work, CronExpression cron, CancellationToken innerCancellation)
        {
            _work = work;
            _cron = cron;
            _innerCancellation = innerCancellation;
        }

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
                await _work.DoWorkAsync(provider, aggregateCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger?.LogError("Scheduled task {0} canceled by runtime", _work);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing scheduled task {0}", _work);
            }
            return _cron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local).HasValue;
        }
    }
}

}
