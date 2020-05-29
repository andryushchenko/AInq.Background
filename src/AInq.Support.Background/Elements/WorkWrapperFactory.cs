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
    internal static class WorkWrapperFactory
    {
        private class SimpleWorkWrapper : ITaskWrapper<object>
        {
            private readonly IWork _work;
            private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal SimpleWorkWrapper(IWork work, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _work = work;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task WorkTask => _completion.Task;

            Task<bool> ITaskWrapper<object>.ExecuteAsync(object argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return Task.FromResult(true);
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

        private class SimpleWorkWrapper<TResult> : ITaskWrapper<object>
        {
            private readonly IWork<TResult> _work;
            private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal SimpleWorkWrapper(IWork<TResult> work, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _work = work;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task<TResult> WorkTask => _completion.Task;

            Task<bool> ITaskWrapper<object>.ExecuteAsync(object argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return Task.FromResult(true);
                _attemptsRemain--;
                try
                {
                    outerCancellation.ThrowIfCancellationRequested();
                    _innerCancellation.ThrowIfCancellationRequested();
                    _completion.TrySetResult(_work.DoWork(provider));
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

        private class AsyncWorkWrapper : ITaskWrapper<object>
        {
            private readonly IAsyncWork _work;
            private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal AsyncWorkWrapper(IAsyncWork work, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _work = work;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task WorkTask => _completion.Task;

            async Task<bool> ITaskWrapper<object>.ExecuteAsync(object argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return true;
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

        private class AsyncWorkWrapper<TResult> : ITaskWrapper<object>
        {
            private readonly IAsyncWork<TResult> _work;
            private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
            private readonly CancellationToken _innerCancellation;
            private int _attemptsRemain;

            internal AsyncWorkWrapper(IAsyncWork<TResult> work, int attemptsCount, CancellationToken innerCancellation)
            {
                if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
                _work = work;
                _innerCancellation = innerCancellation;
                _attemptsRemain = attemptsCount;
            }

            internal Task<TResult> WorkTask => _completion.Task;

            async Task<bool> ITaskWrapper<object>.ExecuteAsync(object argument, IServiceProvider provider, CancellationToken outerCancellation)
            {
                if (_attemptsRemain < 1) return true;
                _attemptsRemain--;
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
                try
                {
                    aggregateCancellation.Token.ThrowIfCancellationRequested();
                    _completion.TrySetResult(await _work.DoWorkAsync(provider, aggregateCancellation.Token));
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

        public static (ITaskWrapper<object> Work, Task Task) CreateWorkWrapper(IWork work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new SimpleWorkWrapper(work, attemptsCount, cancellation);
            return (wrapper, wrapper.WorkTask);
        }

        public static (ITaskWrapper<object> Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IWork<TResult> work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new SimpleWorkWrapper<TResult>(work, attemptsCount, cancellation);
            return (wrapper, wrapper.WorkTask);
        }

        public static (ITaskWrapper<object> Work, Task Task) CreateWorkWrapper(IAsyncWork work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new AsyncWorkWrapper(work, attemptsCount, cancellation);
            return (wrapper, wrapper.WorkTask);
        }

        public static (ITaskWrapper<object> Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IAsyncWork<TResult> work, int attemptsCount = 1, CancellationToken cancellation = default)
        {
            if (attemptsCount < 1) throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            var wrapper = new AsyncWorkWrapper<TResult>(work, attemptsCount, cancellation);
            return (wrapper, wrapper.WorkTask);
        }
    }
}