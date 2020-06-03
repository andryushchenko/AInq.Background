﻿/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Elements
{
    internal static class AccessWrapperFactory
    {
        private class AccessWrapper<TParameter> : ITaskWrapper<TParameter>
        {
            private readonly IAccess<TParameter> _access;
            private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal AccessWrapper(IAccess<TParameter> access, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _access = access;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task AccessTask => _completion.Task;

            Task<bool> ITaskWrapper<TParameter>.ExecuteAsync(TParameter argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return Task.FromResult(true);
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
                    if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested) return Task.FromResult(false);
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    if (_attemptsRemain > 0) return Task.FromResult(false);
                    _completion.TrySetException(ex);
                }
                return Task.FromResult(true);
            }
        }

        private class AccessWrapper<TParameter, TResult> : ITaskWrapper<TParameter>
        {
            private readonly IAccess<TParameter, TResult> _access;
            private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal AccessWrapper(IAccess<TParameter, TResult> access, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _access = access;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task<TResult> AccessTask => _completion.Task;

            Task<bool> ITaskWrapper<TParameter>.ExecuteAsync(TParameter argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return Task.FromResult(true);
                _attemptsRemain--;
                try
                {
                    outerCancellation.ThrowIfCancellationRequested();
                    _innerCancellation.ThrowIfCancellationRequested();
                    _completion.TrySetResult(_access.Access(argument, provider));
                }
                catch (OperationCanceledException ex)
                {
                    if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested) return Task.FromResult(false);
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    if (_attemptsRemain > 0) return Task.FromResult(false);
                    _completion.TrySetException(ex);
                }
                return Task.FromResult(true);
            }
        }

        private class AsyncAccessWrapper<TParameter> : ITaskWrapper<TParameter>
        {
            private readonly IAsyncAccess<TParameter> _access;
            private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal AsyncAccessWrapper(IAsyncAccess<TParameter> access, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _access = access;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task AccessTask => _completion.Task;

            async Task<bool> ITaskWrapper<TParameter>.ExecuteAsync(TParameter argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return true;
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
                    if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested) return false;
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    if (_attemptsRemain > 0) return false;
                    _completion.TrySetException(ex);
                }
                return true;
            }
        }

        private class AsyncAccessWrapper<TParameter, TResult> : ITaskWrapper<TParameter>
        {
            private readonly IAsyncAccess<TParameter, TResult> _access;
            private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal AsyncAccessWrapper(IAsyncAccess<TParameter, TResult> access, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _access = access;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task<TResult> AccessTask => _completion.Task;

            async Task<bool> ITaskWrapper<TParameter>.ExecuteAsync(TParameter argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return true;
                _attemptsRemain--;
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
                try
                {
                    aggregateCancellation.Token.ThrowIfCancellationRequested();
                    _completion.TrySetResult(await _access.AccessAsync(argument, provider, aggregateCancellation.Token));
                }
                catch (OperationCanceledException ex)
                {
                    if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested) return false;
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    if (_attemptsRemain > 0) return false;
                    _completion.TrySetException(ex);
                }
                return true;
            }
        }

        public static (ITaskWrapper<TParameter> Access, Task Task) CreateAccessWrapper<TParameter>(IAccess<TParameter> work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new AccessWrapper<TParameter>(work, attemptsCount, cancellation);
            return (wrapper, wrapper.AccessTask);
        }

        public static (ITaskWrapper<TParameter> Access, Task<TResult> Task) CreateAccessWrapper<TParameter, TResult>(IAccess<TParameter, TResult> work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new AccessWrapper<TParameter, TResult>(work, attemptsCount, cancellation);
            return (wrapper, wrapper.AccessTask);
        }

        public static (ITaskWrapper<TParameter> Access, Task Task) CreateAccessWrapper<TParameter>(IAsyncAccess<TParameter> work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new AsyncAccessWrapper<TParameter>(work, attemptsCount, cancellation);
            return (wrapper, wrapper.AccessTask);
        }

        public static (ITaskWrapper<TParameter> Access, Task<TResult> Task) CreateAccessWrapper<TParameter, TResult>(IAsyncAccess<TParameter, TResult> work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new AsyncAccessWrapper<TParameter, TResult>(work, attemptsCount, cancellation);
            return (wrapper, wrapper.AccessTask);
        }
    }
}