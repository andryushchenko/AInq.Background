﻿// Copyright 2020-2021 Anton Andryushchenko
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

using AInq.Background.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Services
{

/// <summary> Interface for background shared resource access queue </summary>
/// <typeparam name="TResource"> Shared resource type</typeparam>
public interface IAccessQueue<out TResource>
    where TResource : notnull
{
    /// <summary> Max allowed retry on fail attempts </summary>
    int MaxAttempts { get; }

    /// <summary> Enqueue access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> is NULL </exception>
    Task EnqueueAccess(IAccess<TResource> access, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> is NULL </exception>
    Task<TResult> EnqueueAccess<TResult>(IAccess<TResource, TResult> access, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <returns> Access action completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> is NULL </exception>
    Task EnqueueAsyncAccess(IAsyncAccess<TResource> access, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue asynchronous access action </summary>
    /// <param name="access"> Access action instance </param>
    /// <param name="cancellation"> Access cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResult"> Access action result type </typeparam>
    /// <returns> Access action result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="access" /> is NULL </exception>
    Task<TResult> EnqueueAsyncAccess<TResult>(IAsyncAccess<TResource, TResult> access, CancellationToken cancellation = default,
        int attemptsCount = 1);
}

}
