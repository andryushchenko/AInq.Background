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

namespace AInq.Background.Tasks
{

/// <summary> Interface for data processing machine for <see cref="IConveyor{TData,TResult}" /> </summary>
/// <typeparam name="TData"> Input data type </typeparam>
/// <typeparam name="TResult"> Processing result type </typeparam>
public interface IConveyorMachine<in TData, TResult>
    where TData : notnull
{
    /// <summary> Process data asynchronously </summary>
    /// <param name="data"> Data to process </param>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <returns> Processing result task </returns>
    Task<TResult> ProcessDataAsync(TData data, IServiceProvider provider, CancellationToken cancellation = default);
}

}
