// Copyright 2020-2023 Anton Andryushchenko
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

/// <summary> Background data conveyor manager </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public sealed class ConveyorManager<TData, TResult> : TaskManager<IConveyorMachine<TData, TResult>>, IConveyor<TData, TResult>
    where TData : notnull
{
    private readonly int _maxAttempts;

    /// <param name="maxAttempts"> Max allowed retry on fail attempts </param>
    public ConveyorManager(int maxAttempts = int.MaxValue)
        => _maxAttempts = Math.Max(maxAttempts, 1);

    int IConveyor<TData, TResult>.MaxAttempts => _maxAttempts;

    Task<TResult> IConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
    {
        var (wrapper, result) = CreateConveyorDataWrapper<TData, TResult>(data ?? throw new ArgumentNullException(nameof(data)),
            FixAttempts(attemptsCount),
            cancellation);
        AddTask(wrapper);
        return result;
    }

    private int FixAttempts(int attemptsCount)
        => Math.Min(_maxAttempts, Math.Max(1, attemptsCount));
}
