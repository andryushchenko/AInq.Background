/*
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

namespace AInq.Support.Background.WorkElements
{
    internal static class WorkWrapperFactory
    {
        #region Wrappers

        private class SimpleWorkWrapper : IWorkWrapper
        {
            private readonly IWork _work;
            private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
            private readonly CancellationToken _cancellation;

            internal SimpleWorkWrapper(IWork work, CancellationToken cancellation)
            {
                _work = work;
                _cancellation = cancellation;
            }

            internal Task WorkTask => _completion.Task;

            Task IWorkWrapper.DoWorkAsync(IServiceProvider provider, CancellationToken cancellation)
            {
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    _cancellation.ThrowIfCancellationRequested();
                    _work.DoWork(provider);
                    _completion.TrySetResult(true);
                }
                catch (OperationCanceledException ex)
                {
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    _completion.TrySetException(ex);
                }

                return Task.CompletedTask;
            }
        }

        private class SimpleWorkWrapper<TResult> : IWorkWrapper
        {
            private readonly IWork<TResult> _work;
            private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
            private readonly CancellationToken _cancellation;

            internal SimpleWorkWrapper(IWork<TResult> work, CancellationToken cancellation)
            {
                _work = work;
                _cancellation = cancellation;
            }

            internal Task<TResult> WorkTask => _completion.Task;

            Task IWorkWrapper.DoWorkAsync(IServiceProvider provider, CancellationToken cancellation)
            {
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    _cancellation.ThrowIfCancellationRequested();
                    _completion.TrySetResult(_work.DoWork(provider));
                }
                catch (OperationCanceledException ex)
                {
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    _completion.TrySetException(ex);
                }

                return Task.CompletedTask;
            }
        }

        private class AsyncWorkWrapper : IWorkWrapper
        {
            private readonly IAsyncWork _work;
            private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();
            private readonly CancellationToken _cancellation;

            internal AsyncWorkWrapper(IAsyncWork work, CancellationToken cancellation)
            {
                _work = work;
                _cancellation = cancellation;
            }

            internal Task WorkTask => _completion.Task;

            async Task IWorkWrapper.DoWorkAsync(IServiceProvider provider, CancellationToken cancellation)
            {
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellation, cancellation);
                try
                {
                    aggregateCancellation.Token.ThrowIfCancellationRequested();
                    await _work.DoWorkAsync(provider, aggregateCancellation.Token);
                    _completion.TrySetResult(true);
                }
                catch (OperationCanceledException ex)
                {
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    _completion.TrySetException(ex);
                }
            }
        }

        private class AsyncWorkWrapper<TResult> : IWorkWrapper
        {
            private readonly IAsyncWork<TResult> _work;
            private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
            private readonly CancellationToken _cancellation;

            internal AsyncWorkWrapper(IAsyncWork<TResult> work, CancellationToken cancellation)
            {
                _work = work;
                _cancellation = cancellation;
            }

            internal Task<TResult> WorkTask => _completion.Task;

            async Task IWorkWrapper.DoWorkAsync(IServiceProvider provider, CancellationToken cancellation)
            {
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellation, cancellation);
                try
                {
                    aggregateCancellation.Token.ThrowIfCancellationRequested();
                    _completion.TrySetResult(await _work.DoWorkAsync(provider, aggregateCancellation.Token));
                }
                catch (OperationCanceledException ex)
                {
                    _completion.TrySetCanceled(ex.CancellationToken);
                }
                catch (Exception ex)
                {
                    _completion.TrySetException(ex);
                }
            }
        }

        #endregion

        public static (IWorkWrapper Work, Task Task) CreateWorkWrapper(IWork work, CancellationToken cancellation)
        {
            var wrapper = new SimpleWorkWrapper(work, cancellation);
            return (wrapper, wrapper.WorkTask);
        }

        public static (IWorkWrapper Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IWork<TResult> work, CancellationToken cancellation)
        {
            var wrapper = new SimpleWorkWrapper<TResult>(work, cancellation);
            return (wrapper, wrapper.WorkTask);
        }

        public static (IWorkWrapper Work, Task Task) CreateWorkWrapper(IAsyncWork work, CancellationToken cancellation)
        {
            var wrapper = new AsyncWorkWrapper(work, cancellation);
            return (wrapper, wrapper.WorkTask);
        }

        public static (IWorkWrapper Work, Task<TResult> Task) CreateWorkWrapper<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation)
        {
            var wrapper = new AsyncWorkWrapper<TResult>(work, cancellation);
            return (wrapper, wrapper.WorkTask);
        }
    }
}