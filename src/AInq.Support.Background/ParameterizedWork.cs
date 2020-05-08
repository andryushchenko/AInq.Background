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

namespace AInq.Support.Background
{
    public class ParameterizedWork<TParam> : IWork
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

    public class ParameterizedWork<TParam, TResult> : IWork<TResult>
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

    public class ParameterizedAsyncWork<TParam> : IAsyncWork
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

    public class ParameterizedAsyncWork<TParam, TResult> : IAsyncWork<TResult>
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
}