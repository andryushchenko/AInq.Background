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

namespace AInq.Background.Helpers;

/// <summary> Chain of three priority conveyors </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TFirstIntermediate"> Intermediate result type </typeparam>
/// <typeparam name="TSecondIntermediate"> Intermediate result type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
[PublicAPI]
public class PriorityConveyorChain<TData, TFirstIntermediate, TSecondIntermediate, TResult> : IPriorityConveyor<TData, TResult>
    where TData : notnull
    where TFirstIntermediate : notnull
    where TSecondIntermediate : notnull
{
    private readonly IPriorityConveyor<TData, TFirstIntermediate> _first;
    private readonly int _maxAttempts;
    private readonly int _maxPriority;
    private readonly IPriorityConveyor<TFirstIntermediate, TSecondIntermediate> _second;
    private readonly IPriorityConveyor<TSecondIntermediate, TResult> _third;

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IPriorityConveyor<TData, TFirstIntermediate> first,
        IPriorityConveyor<TFirstIntermediate, TSecondIntermediate> second, IPriorityConveyor<TSecondIntermediate, TResult> third)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _third = third ?? throw new ArgumentNullException(nameof(third));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IConveyor<TData, TFirstIntermediate> first, IPriorityConveyor<TFirstIntermediate, TSecondIntermediate> second,
        IPriorityConveyor<TSecondIntermediate, TResult> third)
    {
        _first = new PriorityConveyorEmulator<TData, TFirstIntermediate>(first ?? throw new ArgumentNullException(nameof(first)));
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _third = third ?? throw new ArgumentNullException(nameof(third));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IPriorityConveyor<TData, TFirstIntermediate> first, IConveyor<TFirstIntermediate, TSecondIntermediate> second,
        IPriorityConveyor<TSecondIntermediate, TResult> third)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = new PriorityConveyorEmulator<TFirstIntermediate, TSecondIntermediate>(second ?? throw new ArgumentNullException(nameof(second)));
        _third = third ?? throw new ArgumentNullException(nameof(third));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IPriorityConveyor<TData, TFirstIntermediate> first,
        IPriorityConveyor<TFirstIntermediate, TSecondIntermediate> second, IConveyor<TSecondIntermediate, TResult> third)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _third = new PriorityConveyorEmulator<TSecondIntermediate, TResult>(third ?? throw new ArgumentNullException(nameof(third)));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IConveyor<TData, TFirstIntermediate> first, IConveyor<TFirstIntermediate, TSecondIntermediate> second,
        IPriorityConveyor<TSecondIntermediate, TResult> third)
    {
        _first = new PriorityConveyorEmulator<TData, TFirstIntermediate>(first ?? throw new ArgumentNullException(nameof(first)));
        _second = new PriorityConveyorEmulator<TFirstIntermediate, TSecondIntermediate>(second ?? throw new ArgumentNullException(nameof(second)));
        _third = third ?? throw new ArgumentNullException(nameof(third));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IConveyor<TData, TFirstIntermediate> first, IPriorityConveyor<TFirstIntermediate, TSecondIntermediate> second,
        IConveyor<TSecondIntermediate, TResult> third)
    {
        _first = new PriorityConveyorEmulator<TData, TFirstIntermediate>(first ?? throw new ArgumentNullException(nameof(first)));
        _second = second ?? throw new ArgumentNullException(nameof(second));
        _third = new PriorityConveyorEmulator<TSecondIntermediate, TResult>(third ?? throw new ArgumentNullException(nameof(third)));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    /// <param name="first"> First conveyor </param>
    /// <param name="second"> Second conveyor </param>
    /// <param name="third"> Third conveyor </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="first" />, <paramref name="second" /> or <paramref name="third" /> is NULL </exception>
    public PriorityConveyorChain(IPriorityConveyor<TData, TFirstIntermediate> first, IConveyor<TFirstIntermediate, TSecondIntermediate> second,
        IConveyor<TSecondIntermediate, TResult> third)
    {
        _first = first ?? throw new ArgumentNullException(nameof(first));
        _second = new PriorityConveyorEmulator<TFirstIntermediate, TSecondIntermediate>(second ?? throw new ArgumentNullException(nameof(second)));
        _third = new PriorityConveyorEmulator<TSecondIntermediate, TResult>(third ?? throw new ArgumentNullException(nameof(third)));
        _maxAttempts = Math.Max(_first.MaxAttempts, Math.Max(_second.MaxAttempts, _third.MaxAttempts));
        _maxPriority = Math.Max(_first.MaxPriority, Math.Max(_second.MaxPriority, _third.MaxPriority));
    }

    int IConveyor<TData, TResult>.MaxAttempts => _maxAttempts;

    async Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, int attemptsCount, CancellationToken cancellation)
        => await _third.ProcessDataAsync(await _second.ProcessDataAsync(await _first
                                                                              .ProcessDataAsync(data,
                                                                                  Math.Min(_first.MaxAttempts, attemptsCount),
                                                                                  cancellation)
                                                                              .ConfigureAwait(false),
                                                          Math.Max(1, Math.Min(_second.MaxAttempts, attemptsCount)),
                                                          cancellation)
                                                      .ConfigureAwait(false),
                           Math.Max(1, Math.Min(_third.MaxAttempts, attemptsCount)),
                           cancellation)
                       .ConfigureAwait(false);

    int IPriorityConveyor<TData, TResult>.MaxPriority => _maxPriority;

    async Task<TResult> IPriorityConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, int attemptsCount,
        CancellationToken cancellation)
        => await _third.ProcessDataAsync(await _second.ProcessDataAsync(await _first.ProcessDataAsync(data,
                                                                                        Math.Min(_first.MaxPriority, Math.Max(0, priority)),
                                                                                        Math.Min(_first.MaxAttempts, attemptsCount),
                                                                                        cancellation)
                                                                                    .ConfigureAwait(false),
                                                          Math.Min(_second.MaxPriority, Math.Max(0, priority)),
                                                          Math.Max(1, Math.Min(_second.MaxAttempts, attemptsCount)),
                                                          cancellation)
                                                      .ConfigureAwait(false),
                           Math.Min(_third.MaxPriority, Math.Max(0, priority)),
                           Math.Max(1, Math.Min(_third.MaxAttempts, attemptsCount)),
                           cancellation)
                       .ConfigureAwait(false);
}
