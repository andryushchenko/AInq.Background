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

using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background

{
    public interface IWorkQueue
    {
        Task EnqueueWork(IWork work, CancellationToken cancellation = default, int attemptsCount = 1);
        Task EnqueueWork<TWork>(CancellationToken cancellation = default, int attemptsCount = 1) where TWork : IWork;
        Task<TResult> EnqueueWork<TResult>(IWork<TResult> work, CancellationToken cancellation = default, int attemptsCount = 1);
        Task<TResult> EnqueueWork<TWork, TResult>(CancellationToken cancellation = default, int attemptsCount = 1) where TWork : IWork<TResult>;
        Task EnqueueAsyncWork(IAsyncWork work, CancellationToken cancellation = default, int attemptsCount = 1);
        Task EnqueueAsyncWork<TAsyncWork>(CancellationToken cancellation = default, int attemptsCount = 1) where TAsyncWork : IAsyncWork;
        Task<TResult> EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation = default, int attemptsCount = 1);
        Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(CancellationToken cancellation = default, int attemptsCount = 1) where TAsyncWork : IAsyncWork<TResult>;
    }
}