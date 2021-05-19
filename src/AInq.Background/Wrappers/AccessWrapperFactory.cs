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

using AInq.Background.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

/// <summary> Factory class for creating <see cref="ITaskWrapper{TArgument}" /> for background asyncAccess queues </summary>
public static class AccessWrapperFactory
{
    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asyncAccess action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Wrapper and asyncAccess action completion task </returns>
    public static (ITaskWrapper<TResource> Access, Task Task) CreateAccessWrapper<TResource>(IAccess<TResource> access, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AccessWrapper<TResource>(access ?? throw new ArgumentNullException(nameof(access)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asyncAccess action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Wrapper and asyncAccess action result task </returns>
    public static (ITaskWrapper<TResource> Access, Task<TResult> Task) CreateAccessWrapper<TResource, TResult>(IAccess<TResource, TResult> access,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AccessWrapper<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asynchronous asyncAccess action </summary>
    /// <param name="asyncAccess"> Async access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Wrapper and asyncAccess action completion task </returns>
    public static (ITaskWrapper<TResource> Access, Task Task) CreateAccessWrapper<TResource>(IAsyncAccess<TResource> asyncAccess,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AccessWrapper<TResource>(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asynchronous asyncAccess action </summary>
    /// <param name="asyncAccess"> Async access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Wrapper and asyncAccess action result task </returns>
    public static (ITaskWrapper<TResource> Access, Task<TResult> Task) CreateAccessWrapper<TResource, TResult>(
        IAsyncAccess<TResource, TResult> asyncAccess, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AccessWrapper<TResource, TResult>(asyncAccess ?? throw new ArgumentNullException(nameof(asyncAccess)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    private class AccessWrapper<TResource> : ITaskWrapper<TResource>
        where TResource : notnull
    {
        private readonly IAccess<TResource>? _access;
        private readonly IAsyncAccess<TResource>? _asyncAccess;
        private readonly TaskCompletionSource<bool> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal AccessWrapper(IAsyncAccess<TResource> asyncAccess, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncAccess = asyncAccess;
            _access = null;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal AccessWrapper(IAccess<TResource> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncAccess = null;
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<TResource>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<TResource>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger logger,
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
            try
            {
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                if (_asyncAccess == null)
                    _access!.Access(argument, provider);
                else await _asyncAccess.AccessAsync(argument, provider, aggregateCancellation.Token).ConfigureAwait(false);
                _completion.TrySetResult(true);
            }
            catch (OperationCanceledException ex)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger.LogWarning("Accessing resource {Type} with {Access} canceled by runtime",
                        typeof(TResource),
                        _asyncAccess as object ?? _access);
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accessing resource {Type} with {Access}", typeof(TResource), _asyncAccess as object ?? _access);
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }

    private class AccessWrapper<TResource, TResult> : ITaskWrapper<TResource>
        where TResource : notnull
    {
        private readonly IAccess<TResource, TResult>? _access;
        private readonly IAsyncAccess<TResource, TResult>? _asyncAccess;
        private readonly TaskCompletionSource<TResult> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal AccessWrapper(IAsyncAccess<TResource, TResult> asyncAccess, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncAccess = asyncAccess;
            _access = null;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal AccessWrapper(IAccess<TResource, TResult> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _asyncAccess = null;
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<TResource>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<TResource>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger logger,
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
            try
            {
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(_asyncAccess == null
                    ? _access!.Access(argument, provider)
                    : await _asyncAccess.AccessAsync(argument, provider, aggregateCancellation.Token).ConfigureAwait(false));
            }
            catch (OperationCanceledException ex)
            {
                if (outerCancellation.IsCancellationRequested)
                    logger.LogWarning("Accessing resource {Type} with {Access} canceled by runtime",
                        typeof(TResource),
                        _asyncAccess as object ?? _access);
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accessing resource {Type} with {Access}", typeof(TResource), _asyncAccess as object ?? _access);
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }
}

}
