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

/// <summary> Factory class for creating <see cref="ITaskWrapper{TArgument}" /> for background access queues </summary>
public static class AccessWrapperFactory
{
    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Wrapper and access action completion task </returns>
    public static (ITaskWrapper<TResource> Access, Task Task) CreateAccessWrapper<TResource>(IAccess<TResource> access, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AccessWrapper<TResource>(access ?? throw new ArgumentNullException(nameof(access)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Wrapper and access action result task </returns>
    public static (ITaskWrapper<TResource> Access, Task<TResult> Task) CreateAccessWrapper<TResource, TResult>(IAccess<TResource, TResult> access,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AccessWrapper<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asynchronous access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> Wrapper and access action completion task </returns>
    public static (ITaskWrapper<TResource> Access, Task Task) CreateAccessWrapper<TResource>(IAsyncAccess<TResource> access, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AsyncAccessWrapper<TResource>(access ?? throw new ArgumentNullException(nameof(access)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> for given asynchronous access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Wrapper and access action result task </returns>
    public static (ITaskWrapper<TResource> Access, Task<TResult> Task) CreateAccessWrapper<TResource, TResult>(
        IAsyncAccess<TResource, TResult> access, int attemptsCount = 1, CancellationToken cancellation = default)
        where TResource : notnull
    {
        var wrapper = new AsyncAccessWrapper<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)),
            Math.Max(1, attemptsCount),
            cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    private class AccessWrapper<TResource> : ITaskWrapper<TResource>
        where TResource : notnull
    {
        private readonly IAccess<TResource> _access;
        private readonly TaskCompletionSource<bool> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal AccessWrapper(IAccess<TResource> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<TResource>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<TResource>.IsFaulted => _completion.Task.IsFaulted;

        Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger,
            CancellationToken outerCancellation)
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
                _access.Access(argument, provider);
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
                logger?.LogError(ex, "Error accessing resource {Type} with {Access}", typeof(TResource), _access);
                if (_attemptsRemain > 0)
                    return Task.FromResult(false);
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(true);
        }
    }

    private class AccessWrapper<TResource, TResult> : ITaskWrapper<TResource>
        where TResource : notnull
    {
        private readonly IAccess<TResource, TResult> _access;
        private readonly TaskCompletionSource<TResult> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal AccessWrapper(IAccess<TResource, TResult> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<TResource>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<TResource>.IsFaulted => _completion.Task.IsFaulted;

        Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger,
            CancellationToken outerCancellation)
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
                _completion.TrySetResult(_access.Access(argument, provider));
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
                logger?.LogError(ex, "Error accessing resource {Type} with {Access}", typeof(TResource), _access);
                if (_attemptsRemain > 0)
                    return Task.FromResult(false);
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return Task.FromResult(true);
        }
    }

    private class AsyncAccessWrapper<TResource> : ITaskWrapper<TResource>
        where TResource : notnull
    {
        private readonly IAsyncAccess<TResource> _access;
        private readonly TaskCompletionSource<bool> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal AsyncAccessWrapper(IAsyncAccess<TResource> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<TResource>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<TResource>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger,
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
                await _access.AccessAsync(argument, provider, aggregateCancellation.Token).ConfigureAwait(false);
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
                logger?.LogError(ex, "Error accessing resource {Type} with {Access}", typeof(TResource), _access);
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }

    private class AsyncAccessWrapper<TResource, TResult> : ITaskWrapper<TResource>
        where TResource : notnull
    {
        private readonly IAsyncAccess<TResource, TResult> _access;
        private readonly TaskCompletionSource<TResult> _completion = new();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal AsyncAccessWrapper(IAsyncAccess<TResource, TResult> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;
        bool ITaskWrapper<TResource>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<TResource>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger,
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
                _completion.TrySetResult(await _access.AccessAsync(argument, provider, aggregateCancellation.Token).ConfigureAwait(false));
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
                logger?.LogError(ex, "Error accessing resource {Type} with {Access}", typeof(TResource), _access);
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
