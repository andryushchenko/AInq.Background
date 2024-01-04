// Copyright 2020-2024 Anton Andryushchenko
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

/// <summary> Wrapper around <see cref="IConveyor{TData,TResult}" /> to emulate <see cref="IPriorityConveyor{TData,TResult}" /> </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
[PublicAPI]
public class PriorityConveyorEmulator<TData, TResult> : IPriorityConveyor<TData, TResult>
    where TData : notnull
{
    private readonly IConveyor<TData, TResult> _conveyor;

    /// <param name="conveyor"> Conveyor instance </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="conveyor" /> is NULL </exception>
    public PriorityConveyorEmulator(IConveyor<TData, TResult> conveyor)
        => _conveyor = conveyor ?? throw new ArgumentNullException(nameof(conveyor));

    int IConveyor<TData, TResult>.MaxAttempts => _conveyor.MaxAttempts;

    Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, int attemptsCount, CancellationToken cancellation)
        => _conveyor.ProcessDataAsync(data, attemptsCount, cancellation);

    int IPriorityConveyor<TData, TResult>.MaxPriority => 0;

    Task<TResult> IPriorityConveyor<TData, TResult>.ProcessDataAsync(TData data, int priority, int attemptsCount, CancellationToken cancellation)
        => _conveyor.ProcessDataAsync(data, attemptsCount, cancellation);
}
