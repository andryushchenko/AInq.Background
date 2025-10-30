// Copyright 2020 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace AInq.Background.Services;

/// <summary> Interface for background shared resource access queue with prioritization </summary>
/// <typeparam name="TResource"> Shared resource type </typeparam>
public interface IPriorityAccessQueue<out TResource> : IAccessQueue<TResource>
    where TResource : notnull
{
    /// <summary> Max allowed access priority </summary>
    [PublicAPI]
    int MaxPriority { get; }

    /// <summary> Enqueue access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    Task EnqueueAccess(IAccess<TResource> access, int priority, int attemptsCount = 1, CancellationToken cancellation = default);

    /// <summary> Enqueue access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    [PublicAPI]
    Task<TResult> EnqueueAccess<TResult>(IAccess<TResource, TResult> access, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <returns> Access action completion task </returns>
    [PublicAPI]
    Task EnqueueAsyncAccess(IAsyncAccess<TResource> access, int priority, int attemptsCount = 1, CancellationToken cancellation = default);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="priority"> Access action priority </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    [PublicAPI]
    Task<TResult> EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, int priority, int attemptsCount = 1,
        CancellationToken cancellation = default);
}
