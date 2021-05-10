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

/// <summary> Chain of two priority conveyors </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TIntermediate"> Intermediate result type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public class PriorityConveyorChain<TData, TIntermediate, TResult> : IPriorityConveyor<TData, TResult>
    where TData : notnull
    where TIntermediate : notnull
{
    private readonly IPriorityConveyor<TData, TIntermediate> _first;
    private readonly int _maxAttempts;
    private readonly int _maxPriority;
    private readonly IPriorityConveyor<TIntermediate, TResult> _second;

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" /> or <paramref name="second" /> is NULL </exception>
    public PriorityConveyorChain(IPriorityConveyor<TData, TIntermediate> first, IPriorityConveyor<TIntermediate, TResult> second)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _maxPriority = Math.Max(_first.MaxPriority, _second.MaxPriority);
        _maxAttempts = Math.Max(_first.MaxAttempts, _second.MaxAttempts);
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" /> or <paramref name="second" /> is NULL </exception>
    public PriorityConveyorChain(IConveyor<TData, TIntermediate> first, IPriorityConveyor<TIntermediate, TResult> second)
    {
        _first = new PriorityConveyorEmulator<TData, TIntermediate>(first);
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _maxPriority = Math.Max(_first.MaxPriority, _second.MaxPriority);
        _maxAttempts = Math.Max(_first.MaxAttempts, _second.MaxAttempts);
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" /> or <paramref name="second" /> is NULL </exception>
    public PriorityConveyorChain(IPriorityConveyor<TData, TIntermediate> first, IConveyor<TIntermediate, TResult> second)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = new PriorityConveyorEmulator<TIntermediate, TResult>(second);
        _maxPriority = Math.Max(_first.MaxPriority, _second.MaxPriority);
        _maxAttempts = Math.Max(_first.MaxAttempts, _second.MaxAttempts);
    }

    int IConveyor<TData, TResult>.MaxAttempts => _maxAttempts;

    async Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
        => await _second.ProcessDataAsync(
                            await _first.ProcessDataAsync(data, cancellation, Math.Min(_first.MaxAttempts, attemptsCount)).ConfigureAwait(false),
                            cancellation,
                            Math.Min(_second.MaxAttempts, attemptsCount))
                        .ConfigureAwait(false);

    int IPriorityConveyor<TData, TResult>.MaxPriority => _maxPriority;

    async Task<TResult> IPriorityConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, CancellationToken cancellation,
        int attemptsCount)
        => await _second.ProcessDataAsync(await _first.ProcessDataAsync(data,
                                                          Math.Min(_first.MaxPriority, priority),
                                                          cancellation,
                                                          Math.Min(_first.MaxAttempts, attemptsCount))
                                                      .ConfigureAwait(false),
                            Math.Min(_second.MaxPriority, priority),
                            cancellation,
                            Math.Min(_second.MaxAttempts, attemptsCount))
                        .ConfigureAwait(false);
}

}
