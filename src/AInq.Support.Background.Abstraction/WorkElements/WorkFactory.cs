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
    public static class WorkFactory
    {

        private class SimpleWork : IWork
        {
            private readonly Action<IServiceProvider> _work;

            internal SimpleWork(Action<IServiceProvider> work)
                => _work = work ?? throw new ArgumentNullException(nameof(work));

            void IWork.DoWork(IServiceProvider serviceProvider)
                => _work.Invoke(serviceProvider);
        }

        private class SimpleWork<TResult> : IWork<TResult>
        {
            private readonly Func<IServiceProvider, TResult> _work;

            internal SimpleWork(Func<IServiceProvider, TResult> work)
                => _work = work ?? throw new ArgumentNullException(nameof(work));

            TResult IWork<TResult>.DoWork(IServiceProvider serviceProvider)
                => _work.Invoke(serviceProvider);
        }

        private class SimpleAsyncWork : IAsyncWork
        {
            private readonly Func<IServiceProvider, CancellationToken, Task> _work;

            internal SimpleAsyncWork(Func<IServiceProvider, CancellationToken, Task> work)
                => _work = work ?? throw new ArgumentNullException(nameof(work));

            async Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
                => await _work.Invoke(serviceProvider, cancellation);
        }

        private class SimpleAsyncWork<TResult> : IAsyncWork<TResult>
        {
            private readonly Func<IServiceProvider, CancellationToken, Task<TResult>> _work;

            internal SimpleAsyncWork(Func<IServiceProvider, CancellationToken, Task<TResult>> work)
                => _work = work ?? throw new ArgumentNullException(nameof(work));

            async Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
                => await _work.Invoke(serviceProvider, cancellation);
        }

        private class ParameterizedWork<TParam> : IWork
        {
            private readonly Action<IServiceProvider, TParam> _work;
            private readonly TParam _param;

            internal ParameterizedWork(Action<IServiceProvider, TParam> work, TParam param)
            {
                _work = work ?? throw new ArgumentNullException(nameof(work));
                _param = param;
            }

            void IWork.DoWork(IServiceProvider serviceProvider)
                => _work.Invoke(serviceProvider, _param);
        }

        private class ParameterizedWork<TParam, TResult> : IWork<TResult>
        {
            private readonly Func<IServiceProvider, TParam, TResult> _work;
            private readonly TParam _param;

            internal ParameterizedWork(Func<IServiceProvider, TParam, TResult> work, TParam param)
            {
                _work = work ?? throw new ArgumentNullException(nameof(work));
                _param = param;
            }

            TResult IWork<TResult>.DoWork(IServiceProvider serviceProvider)
                => _work.Invoke(serviceProvider, _param);
        }

        private class ParameterizedAsyncWork<TParam> : IAsyncWork
        {
            private readonly Func<IServiceProvider, TParam, CancellationToken, Task> _work;
            private readonly TParam _param;

            internal ParameterizedAsyncWork(Func<IServiceProvider, TParam, CancellationToken, Task> work, TParam param)
            {
                _work = work ?? throw new ArgumentNullException(nameof(work));
                _param = param;
            }

            async Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
                => await _work.Invoke(serviceProvider, _param, cancellation);
        }

        private class ParameterizedAsyncWork<TParam, TResult> : IAsyncWork<TResult>
        {
            private readonly Func<IServiceProvider, TParam, CancellationToken, Task<TResult>> _work;
            private readonly TParam _param;

            internal ParameterizedAsyncWork(Func<IServiceProvider, TParam, CancellationToken, Task<TResult>> work, TParam param)
            {
                _work = work ?? throw new ArgumentNullException(nameof(work));
                _param = param;
            }

            async Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
                => await _work.Invoke(serviceProvider, _param, cancellation);
        }

        public static IWork CreateWork(Action work)
            => work != null
                ? new SimpleWork(provider => work.Invoke())
                : throw new ArgumentNullException(nameof(work));

        public static IWork<TResult> CreateWork<TResult>(Func<TResult> work)
            => work != null
                ? new SimpleWork<TResult>(provider => work.Invoke())
                : throw new ArgumentNullException(nameof(work));

        public static IWork CreateWork(Action<IServiceProvider> work)
            => new SimpleWork(work);

        public static IWork<TResult> CreateWork<TResult>(Func<IServiceProvider, TResult> work)
            => new SimpleWork<TResult>(work);

        public static IWork CreateWork<TParam>(Action<TParam> work, TParam param)
            => work != null
                ? new ParameterizedWork<TParam>((provider, arg) => work.Invoke(arg), param)
                : throw new ArgumentNullException(nameof(work));

        public static IWork<TResult> CreateWork<TParam, TResult>(Func<TParam, TResult> work, TParam param)
            => work != null
                ? new ParameterizedWork<TParam, TResult>((provider, arg) => work.Invoke(arg), param)
                : throw new ArgumentNullException(nameof(work));

        public static IWork CreateWork<TParam>(Action<IServiceProvider, TParam> work, TParam param)
            => new ParameterizedWork<TParam>(work, param);

        public static IWork<TResult> CreateWork<TParam, TResult>(Func<IServiceProvider, TParam, TResult> work, TParam param)
            => new ParameterizedWork<TParam, TResult>(work, param);

        public static IAsyncWork CreateWork(Func<CancellationToken, Task> work)
            => work != null
                ? new SimpleAsyncWork((provider, token) => work.Invoke(token))
                : throw new ArgumentNullException(nameof(work));

        public static IAsyncWork<TResult> CreateWork<TResult>(Func<CancellationToken, Task<TResult>> work)
            => work != null
                ? new SimpleAsyncWork<TResult>((provider, token) => work.Invoke(token))
                : throw new ArgumentNullException(nameof(work));

        public static IAsyncWork CreateWork(Func<IServiceProvider, CancellationToken, Task> work)
            => new SimpleAsyncWork(work);

        public static IAsyncWork<TResult> CreateWork<TResult>(Func<IServiceProvider, CancellationToken, Task<TResult>> work)
            => new SimpleAsyncWork<TResult>(work);

        public static IAsyncWork CreateWork<TParam>(Func<TParam, CancellationToken, Task> work, TParam param)
            => work != null
                ? new ParameterizedAsyncWork<TParam>((provider, arg, token) => work.Invoke(arg, token), param)
                : throw new ArgumentNullException(nameof(work));

        public static IAsyncWork<TResult> CreateWork<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> work, TParam param)
            => work != null
                ? new ParameterizedAsyncWork<TParam, TResult>((provider, arg, token) => work.Invoke(arg, token), param)
                : throw new ArgumentNullException(nameof(work));

        public static IAsyncWork CreateWork<TParam>(Func<IServiceProvider, TParam, CancellationToken, Task> work, TParam param)
            => new ParameterizedAsyncWork<TParam>(work, param);

        public static IAsyncWork<TResult> CreateWork<TParam, TResult>(Func<IServiceProvider, TParam, CancellationToken, Task<TResult>> work, TParam param)
            => new ParameterizedAsyncWork<TParam, TResult>(work, param);
    }
}