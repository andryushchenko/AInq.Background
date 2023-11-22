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

namespace AInq.Background.Services;

/// <summary> Interface for background data processing conveyor </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public interface IConveyor<in TData, TResult>
    where TData : notnull
{
    /// <summary> Max allowed retry on fail attempts </summary>
    [PublicAPI]
    int MaxAttempts { get; }

    /// <summary> Process data asynchronously in queue </summary>
    /// <param name="data"> Data to process </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <returns> Processing result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="data" /> is NULL </exception>
    [PublicAPI]
    Task<TResult> ProcessDataAsync(TData data, int attemptsCount = 1, CancellationToken cancellation = default);
}
