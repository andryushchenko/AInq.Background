﻿// Copyright 2020 Anton Andryushchenko
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

/// <summary> Interface for background work queue </summary>
public interface IWorkQueue
{
    /// <summary> Max allowed retry on fail attempts </summary>
    int MaxAttempts { get; }

    /// <summary> Enqueue background work </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    Task EnqueueWork(IWork work, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue background work </summary>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    Task EnqueueWork<TWork>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TWork : IWork;

    /// <summary> Enqueue background work </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    Task<TResult> EnqueueWork<TResult>(IWork<TResult> work, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue background work </summary>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    Task<TResult> EnqueueWork<TWork, TResult>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TWork : IWork<TResult>;

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <returns> Work completion task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    Task EnqueueAsyncWork(IAsyncWork work, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <returns> Work completion task </returns>
    Task EnqueueAsyncWork<TAsyncWork>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TAsyncWork : IAsyncWork;

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="work"> Work instance </param>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="work" /> is NULL </exception>
    Task<TResult> EnqueueAsyncWork<TResult>(IAsyncWork<TResult> work, CancellationToken cancellation = default, int attemptsCount = 1);

    /// <summary> Enqueue asynchronous background work </summary>
    /// <param name="cancellation"> Work cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <typeparam name="TAsyncWork"> Work type </typeparam>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> Work result task </returns>
    Task<TResult> EnqueueAsyncWork<TAsyncWork, TResult>(CancellationToken cancellation = default, int attemptsCount = 1)
        where TAsyncWork : IAsyncWork<TResult>;
}

}