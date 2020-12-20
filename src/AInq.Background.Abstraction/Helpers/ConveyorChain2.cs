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

using AInq.Background.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Helpers
{

/// <summary> Chain of two conveyors </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TIntermediate"> Intermediate result type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public class ConveyorChain<TData, TIntermediate, TResult> : IConveyor<TData, TResult>
    where TData : notnull
    where TIntermediate : notnull
{
    private readonly IConveyor<TData, TIntermediate> _first;
    private readonly IConveyor<TIntermediate, TResult> _second;

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" /> or <paramref name="second" /> is NULL </exception>
    public ConveyorChain(IConveyor<TData, TIntermediate> first, IConveyor<TIntermediate, TResult> second)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
    }

    int IConveyor<TData, TResult>.MaxAttempts => Math.Max(_first.MaxAttempts, _second.MaxAttempts);

    async Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
        => await _second.ProcessDataAsync(await _first.ProcessDataAsync(data, cancellation, Math.Min(_first.MaxAttempts, attemptsCount))
                                                      .ConfigureAwait(false),
                            cancellation,
                            Math.Min(_second.MaxAttempts, attemptsCount))
                        .ConfigureAwait(false);
}

}
