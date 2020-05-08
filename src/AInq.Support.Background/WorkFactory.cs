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
    public static class WorkFactory
    {
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