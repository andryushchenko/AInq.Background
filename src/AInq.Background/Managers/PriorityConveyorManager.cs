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

using static AInq.Background.Wrappers.ConveyorDataWrapperFactory;

namespace AInq.Background.Managers;

/// <summary> Background data conveyor manager with numeric prioritization </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public sealed class PriorityConveyorManager<TData, TResult> : PriorityTaskManager<IConveyorMachine<TData, TResult>>, IPriorityConveyor<TData, TResult>
    where TData : notnull
{
    private readonly int _maxAttempts;

    /// <param name="maxPriority"> Max allowed work priority </param>
    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public PriorityConveyorManager(int maxPriority = 100, int maxAttempts = int.MaxValue) : base(maxPriority)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IPriorityConveyor<TData, TResult>.MaxPriority => MaxPriority;

    Task<TResult> IPriorityConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, int attemptsCount, CancellationToken cancellation)
    {
        var (wrapper, result) = CreateConveyorDataWrapper<TData, TResult>(data ?? throw new ArgumentNullException(nameof(data)),
            FixAttempts(attemptsCount),
            cancellation);
        AddTask(wrapper, priority);
        return result;
    }

    int IConveyor<TData, TResult>.MaxAttempts => _maxAttempts;

    Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, int attemptsCount, CancellationToken cancellation)
    {
        var (wrapper, result) = CreateConveyorDataWrapper<TData, TResult>(data ?? throw new ArgumentNullException(nameof(data)),
            FixAttempts(attemptsCount),
            cancellation);
        AddTask(wrapper, 0);
        return result;
    }

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}
