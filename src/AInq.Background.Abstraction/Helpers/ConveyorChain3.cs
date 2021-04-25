// Copyright 2021 Anton Andryushchenko
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

/// <summary> Chain of three conveyors </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TFirstIntermediate"> Intermediate result type </typeparam>
/// <typeparam name="TSecondIntermediate"> Intermediate result type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public class ConveyorChain<TData, TFirstIntermediate, TSecondIntermediate, TResult> : IConveyor<TData, TResult>
    where TData : notnull
    where TFirstIntermediate : notnull
    where TSecondIntermediate : notnull
{
    private readonly IConveyor<TData, TFirstIntermediate> _first;
    private readonly IConveyor<TFirstIntermediate, TSecondIntermediate> _second;
    private readonly IConveyor<TSecondIntermediate, TResult> _third;

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public ConveyorChain(IConveyor<TData, TFirstIntermediate> first, IConveyor<TFirstIntermediate, TSecondIntermediate> second,
        IConveyor<TSecondIntermediate, TResult> third)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _third = third ?? throw new ArgumentNullException(nameof(third));
    }

    int IConveyor<TData, TResult>.MaxAttempts => Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));

    async Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
        => await _third.ProcessDataAsync(await _second.ProcessDataAsync(await _first.ProcessDataAsync(data,
                                                                                        cancellation,
                                                                                        Math.Min(_first.MaxAttempts, attemptsCount))
                                                                                    .ConfigureAwait(false),
                                                          cancellation,
                                                          Math.Min(_second.MaxAttempts, attemptsCount))
                                                      .ConfigureAwait(false),
                           cancellation,
                           Math.Min(_third.MaxAttempts, attemptsCount))
                       .ConfigureAwait(false);
}

}
