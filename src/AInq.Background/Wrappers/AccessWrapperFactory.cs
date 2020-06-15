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

internal static class AccessWrapperFactory
{
    private class AccessWrapper<TResource> : ITaskWrapper<TResource>
    {
        private readonly IAccess<TResource> _access;
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal AccessWrapper(IAccess<TResource> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
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
    {
        private readonly IAccess<TResource, TResult> _access;
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal AccessWrapper(IAccess<TResource, TResult> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;

        Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
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
    {
        private readonly IAsyncAccess<TResource> _access;
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal AsyncAccessWrapper(IAsyncAccess<TResource> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
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
                await _access.AccessAsync(argument, provider, aggregateCancellation.Token);
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
    {
        private readonly IAsyncAccess<TResource, TResult> _access;
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly CancellationToken _innerCancellation;
        private CancellationTokenRegistration _cancellationRegistration;
        private int _attemptsRemain;

        internal AsyncAccessWrapper(IAsyncAccess<TResource, TResult> access, int attemptsCount, CancellationToken innerCancellation)
        {
            _access = access;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> AccessTask => _completion.Task;
        bool ITaskWrapper<TResource>.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> ITaskWrapper<TResource>.ExecuteAsync(TResource argument, IServiceProvider provider, ILogger? logger, CancellationToken outerCancellation)
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
                _completion.TrySetResult(await _access.AccessAsync(argument, provider, aggregateCancellation.Token));
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

    public static (ITaskWrapper<TResource> Access, Task Task) CreateAccessWrapper<TResource>(IAccess<TResource> access, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new AccessWrapper<TResource>(access ?? throw new ArgumentNullException(nameof(access)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    public static (ITaskWrapper<TResource> Access, Task<TResult> Task) CreateAccessWrapper<TResource, TResult>(IAccess<TResource, TResult> access, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new AccessWrapper<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    public static (ITaskWrapper<TResource> Access, Task Task) CreateAccessWrapper<TResource>(IAsyncAccess<TResource> access, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new AsyncAccessWrapper<TResource>(access ?? throw new ArgumentNullException(nameof(access)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.AccessTask);
    }

    public static (ITaskWrapper<TResource> Access, Task<TResult> Task) CreateAccessWrapper<TResource, TResult>(IAsyncAccess<TResource, TResult> access, int attemptsCount = 1, CancellationToken cancellation = default)
    {
        var wrapper = new AsyncAccessWrapper<TResource, TResult>(access ?? throw new ArgumentNullException(nameof(access)), Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.AccessTask);
    }
}

}
