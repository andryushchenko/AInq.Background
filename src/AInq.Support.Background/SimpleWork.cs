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
    internal class SimpleWork : IWork
    {
        private readonly Action<IServiceProvider> _work;

        internal SimpleWork(Action<IServiceProvider> work)
        {
            _work = work ?? throw new ArgumentNullException(nameof(work));
        }

        void IWork.DoWork(IServiceProvider serviceProvider)
        {
            _work.Invoke(serviceProvider);
        }
    }

    internal class SimpleWork<TResult> : IWork<TResult>
    {
        private readonly Func<IServiceProvider, TResult> _work;

        internal SimpleWork(Func<IServiceProvider, TResult> work)
        {
            _work = work ?? throw new ArgumentNullException(nameof(work));
        }

        TResult IWork<TResult>.DoWork(IServiceProvider serviceProvider)
        {
            return _work.Invoke(serviceProvider);
        }
    }

    internal class SimpleAsyncWork : IAsyncWork
    {
        private readonly Func<IServiceProvider, CancellationToken, Task> _work;

        internal SimpleAsyncWork(Func<IServiceProvider, CancellationToken, Task> work)
        {
            _work = work ?? throw new ArgumentNullException(nameof(work));
        }

        async Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            await _work.Invoke(serviceProvider, cancellation);
        }
    }

    internal class SimpleAsyncWork<TResult> : IAsyncWork<TResult>
    {
        private readonly Func<IServiceProvider, CancellationToken, Task<TResult>> _work;

        internal SimpleAsyncWork(Func<IServiceProvider, CancellationToken, Task<TResult>> work)
        {
            _work = work ?? throw new ArgumentNullException(nameof(work));
        }

        async Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            return await _work.Invoke(serviceProvider, cancellation);
        }
    }
}