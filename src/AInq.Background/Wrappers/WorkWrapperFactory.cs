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

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

internal static class WorkWrapperFactory
{
    private class WorkWrapper : ITaskWrapper<object?>
    {
        private readonly IWork _work;
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal WorkWrapper(IWork work, int attemptsCount, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task WorkTask => _completion.Task;
        bool ITaskWrapper<object?>.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> ITaskWrapper<object?>.ExecuteAsync(object? argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            if (_attemptsRemain < 1)
            {
                _completion.TrySetException(new InvalidOperationException("No attempts left"));
                _cancellationRegistration.Dispose();
                _cancellationRegistration = default;
                return Task.FromResult(true);
            }
            _attemptsRemain--;
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _work.DoWork(provider);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException ex)
            {
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return Task.FromResult(false);
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing queued work {Work}", _work);
                if (_attemptsRemain > 0)
                    return Task.FromResult(false);
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(true);
        }
    }

    private class WorkWrapper<TResult> : ITaskWrapper<object?>
    {
        private readonly IWork<TResult> _work;
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal WorkWrapper(IWork<TResult> work, int attemptsCount, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> WorkTask => _completion.Task;
        bool ITaskWrapper<object?>.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> ITaskWrapper<object?>.ExecuteAsync(object? argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            if (_attemptsRemain < 1)
            {
                _completion.TrySetException(new InvalidOperationException("No attempts left"));
                _cancellationRegistration.Dispose();
                _cancellationRegistration = default;
                return Task.FromResult(true);
            }
            _attemptsRemain--;
            try
            {
                outerCancellation.ThrowIfCancellationRequested();
                _innerCancellation.ThrowIfCancellationRequested();
                _completion.TrySetResult(_work.DoWork(provider));
            }
            catch (OperationCanceledException ex)
            {
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return Task.FromResult(false);
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing queued work {Work}", _work);
                if (_attemptsRemain > 0)
                    return Task.FromResult(false);
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(true);
        }
    }

    private class AsyncWorkWrapper : ITaskWrapper<object?>
    {
        private readonly IAsyncWork _work;
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal AsyncWorkWrapper(IAsyncWork work, int attemptsCount, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task WorkTask => _completion.Task;
        bool ITaskWrapper<object?>.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> ITaskWrapper<object?>.ExecuteAsync(object? argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            if (_attemptsRemain < 1)
            {
                _completion.TrySetException(new InvalidOperationException("No attempts left"));
                _cancellationRegistration.Dispose();
                _cancellationRegistration = default;
                return true;
            }
            _attemptsRemain--;
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                await _work.DoWorkAsync(provider, aggregateCancellation.Token);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException ex)
            {
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing queued work {Work}", _work);
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }

    private class AsyncWorkWrapper<TResult> : ITaskWrapper<object?>
    {
        private readonly IAsyncWork<TResult> _work;
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal AsyncWorkWrapper(IAsyncWork<TResult> work, int attemptsCount, CancellationToken innerCancellation)
        {
            _work = work;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> WorkTask => _completion.Task;
        bool ITaskWrapper<object?>.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> ITaskWrapper<object?>.ExecuteAsync(object? argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
        {
            if (_attemptsRemain < 1)
            {
                _completion.TrySetException(new InvalidOperationException("No attempts left"));
                _cancellationRegistration.Dispose();
                _cancellationRegistration = default;
                return true;
            }
            _attemptsRemain--;
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(await _work.DoWorkAsync(provider, aggregateCancellation.Token));
            }
            catch (OperationCanceledException ex)
            {
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing queued work {Work}", _work);
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }

    public static (ITaskWrapper<object?> Work, Task Task) CreateWorkWrapper(IWork work, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new WorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    public static (ITaskWrapper<object?> Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IWork<TResult> work, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new WorkWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    public static (ITaskWrapper<object?> Work, Task Task) CreateWorkWrapper(IAsyncWork work, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new AsyncWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    public static (ITaskWrapper<object?> Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IAsyncWork<TResult> work, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new AsyncWorkWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }
}

}
