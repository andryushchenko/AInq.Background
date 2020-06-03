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
    public static class AccessFactory
    {
        private class Access<TParameter> : IAccess<TParameter>
        {
            private readonly Action<TParameter, IServiceProvider> _access;

            internal Access(Action<TParameter, IServiceProvider> access)
            {
                _access = access ?? throw new ArgumentNullException(nameof(access));
            }

            void IAccess<TParameter>.Access(TParameter parameter, IServiceProvider serviceProvider)
                => _access.Invoke(parameter, serviceProvider);
        }

        private class Access<TParameter, TResult> : IAccess<TParameter, TResult>
        {
            private readonly Func<TParameter, IServiceProvider, TResult> _access;

            internal Access(Func<TParameter, IServiceProvider, TResult> access)
            {
                _access = access ?? throw new ArgumentNullException(nameof(access));
            }

            TResult IAccess<TParameter, TResult>.Access(TParameter parameter, IServiceProvider serviceProvider)
                => _access.Invoke(parameter, serviceProvider);
        }

        private class AsyncAccess<TParameter> : IAsyncAccess<TParameter>
        {
            private readonly Func<TParameter, IServiceProvider, CancellationToken, Task> _access;

            internal AsyncAccess(Func<TParameter, IServiceProvider, CancellationToken, Task> access)
            {
                _access = access ?? throw new ArgumentNullException(nameof(access));
            }

            async Task IAsyncAccess<TParameter>.AccessAsync(TParameter parameter, IServiceProvider serviceProvider, CancellationToken cancellation)
                => await _access.Invoke(parameter, serviceProvider, cancellation);
        }

        private class AsyncAccess<TParameter, TResult> : IAsyncAccess<TParameter, TResult>
        {
            private readonly Func<TParameter, IServiceProvider, CancellationToken, Task<TResult>> _access;

            internal AsyncAccess(Func<TParameter, IServiceProvider, CancellationToken, Task<TResult>> access)
            {
                _access = access ?? throw new ArgumentNullException(nameof(access));
            }

            async Task<TResult> IAsyncAccess<TParameter, TResult>.AccessAsync(TParameter parameter, IServiceProvider serviceProvider, CancellationToken cancellation)
                => await _access.Invoke(parameter, serviceProvider, cancellation);
        }

        public static IAccess<TParameter> CreateAccess<TParameter>(Action<TParameter> access)
            => access != null
                ? new Access<TParameter>((parameter, provider) => access.Invoke(parameter))
                : throw new ArgumentNullException(nameof(access));

        public static IAccess<TParameter, TResult> CreateAccess<TParameter, TResult>(Func<TParameter, TResult> access)
            => access != null
                ? new Access<TParameter, TResult>((parameter, provider) => access.Invoke(parameter))
                : throw new ArgumentNullException(nameof(access));

        public static IAccess<TParameter> CreateAccess<TParameter>(Action<TParameter, IServiceProvider> access)
            => new Access<TParameter>(access ?? throw new ArgumentNullException(nameof(access)));

        public static IAccess<TParameter, TResult> CreateAccess<TParameter, TResult>(Func<TParameter, IServiceProvider, TResult> access)
            => new Access<TParameter, TResult>(access ?? throw new ArgumentNullException(nameof(access)));

        public static IAsyncAccess<TParameter> CreateAccess<TParameter>(Func<TParameter, CancellationToken, Task> access)
            => access != null
                ? new AsyncAccess<TParameter>((parameter, provider, token) => access.Invoke(parameter, token))
                : throw new ArgumentNullException(nameof(access));

        public static IAsyncAccess<TParameter, TResult> CreateAccess<TParameter, TResult>(Func<TParameter, CancellationToken, Task<TResult>> access)
            => access != null
                ? new AsyncAccess<TParameter, TResult>((parameter, provider, token) => access.Invoke(parameter, token))
                : throw new ArgumentNullException(nameof(access));

        public static IAsyncAccess<TParameter> CreateAccess<TParameter>(Func<TParameter, IServiceProvider, CancellationToken, Task> access)
            => new AsyncAccess<TParameter>(access ?? throw new ArgumentNullException(nameof(access)));

        public static IAsyncAccess<TParameter, TResult> CreateAccess<TParameter, TResult>(Func<TParameter, IServiceProvider, CancellationToken, Task<TResult>> access)
            => new AsyncAccess<TParameter, TResult>(access ?? throw new ArgumentNullException(nameof(access)));
    }
}