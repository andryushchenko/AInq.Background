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

using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background
{

public interface IAccessQueue<out TResource>
{
    int MaxAttempts { get; }
    Task EnqueueAccess(IAccess<TResource> access, CancellationToken cancellation = default, int attemptsCount = 1);

    Task EnqueueAccess<TAccess>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TAccess : IAccess<TResource>;

    Task<TResult> EnqueueAccess<TResult>(IAccess<TResource, TResult> access, CancellationToken cancellation = default, int attemptsCount = 1);

    Task<TResult> EnqueueAccess<TAccess, TResult>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TAccess : IAccess<TResource, TResult>;

    Task EnqueueAsyncAccess(IAsyncAccess<TResource> access, CancellationToken cancellation = default, int attemptsCount = 1);

    Task EnqueueAsyncAccess<TAsyncAccess>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TAsyncAccess : IAsyncAccess<TResource>;

    Task<TResult> EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, CancellationToken cancellation = default, int attemptsCount = 1);

    Task<TResult> EnqueueAsyncAccess<TAsyncAccess, TResult>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TAsyncAccess : IAsyncAccess<TResource, TResult>;
}

}
