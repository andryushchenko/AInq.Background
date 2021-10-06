// Copyright 2020-2021 Anton Andryushchenko
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

namespace AInq.Background.Wrappers;

/// <summary> Factory class for creating <see cref="ITaskWrapper{TArgument}" /> for background asyncWork queues </summary>
public static class WorkWrapperFactory
{
    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asyncWork </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Wrapper and asyncWork completion task </returns>
    public static (ITaskWrapper<object?> Work, Task Task) CreateWorkWrapper(IWork work, int attemptsCount = 1,
        CancellationToken cancellation = default)
    {
        var wrapper = new WorkWrapper(work ?? throw new ArgumentNullException(nameof(work)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asyncWork </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Wrapper and asyncWork result task </returns>
    public static (ITaskWrapper<object?> Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IWork<TResult> work, int attemptsCount = 1,
        CancellationToken cancellation = default)
    {
        var wrapper = new WorkWrapper<TResult>(work ?? throw new ArgumentNullException(nameof(work)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asynchronous asyncWork </summary>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <returns> Wrapper and asyncWork completion task </returns>
    public static (ITaskWrapper<object?> Work, Task Task) CreateWorkWrapper(IAsyncWork asyncWork, int attemptsCount = 1,
        CancellationToken cancellation = default)
    {
        var wrapper = new WorkWrapper(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asynchronous asyncWork </summary>
    /// <param name="asyncWork"> Async work instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Wrapper and asyncWork result task </returns>
    public static (ITaskWrapper<object?> Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IAsyncWork<TResult> asyncWork, int attemptsCount = 1,
        CancellationToken cancellation = default)
    {
        var wrapper = new WorkWrapper<TResult>(asyncWork ?? throw new ArgumentNullException(nameof(asyncWork)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.WorkTask);
    }

    private class WorkWrapper : ITaskWrapper<object?>
    {
        private readonly IAsyncWork? _asyncWork;
        private readonly TaskCompletionSource<bool> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private readonly IWork? _work;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal WorkWrapper(IAsyncWork asyncWork, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncWork = asyncWork;
            _work = null;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal WorkWrapper(IWork work, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncWork = null;
            _work = work;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task WorkTask => _completion.Task;
        bool ITaskWrapper<object?>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<object?>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<object?>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<object?>.ExecuteAsync(object? argument, IServiceProvider provider, ILogger logger,
            CancellationToken outerCancellation)
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
                if (_asyncWork == null)
                    _work!.DoWork(provider);
                else await _asyncWork.DoWorkAsync(provider, aggregateCancellation.Token).ConfigureAwait(false);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException ex)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger.LogWarning("Queued work {Work} canceled by runtime", _asyncWork as object ?? _work);
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }

    private class WorkWrapper<TResult> : ITaskWrapper<object?>
    {
        private readonly IAsyncWork<TResult>? _asyncWork;
        private readonly TaskCompletionSource<TResult> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private readonly IWork<TResult>? _work;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal WorkWrapper(IAsyncWork<TResult> asyncWork, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncWork = asyncWork;
            _work = null;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal WorkWrapper(IWork<TResult> work, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncWork = null;
            _work = work;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> WorkTask => _completion.Task;
        bool ITaskWrapper<object?>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<object?>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<object?>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<object?>.ExecuteAsync(object? argument, IServiceProvider provider, ILogger logger,
            CancellationToken outerCancellation)
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
                _completion.TrySetResult(_asyncWork == null
                    ? _work!.DoWork(provider)
                    : await _asyncWork.DoWorkAsync(provider, aggregateCancellation.Token).ConfigureAwait(false));
            }
            catch (OperationCanceledException ex)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger.LogWarning("Queued work {Work} canceled by runtime", _asyncWork as object ?? _work);
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }
}
