// Copyright 2020 Anton Andryushchenko
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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background
{

/// <summary> Helper class for <see cref="IConveyor{TData,TResult}"/> and <see cref="IPriorityConveyor{TData,TResult}"/> </summary>
public static class ConveyorHelper
{
    /// <summary> Processes data using registered conveyor with giver <paramref name="priority"/> (if supported) </summary>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="data"> Data to process </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="priority"> Operation priority </param>
    /// <typeparam name="TData"> Input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Processing result task </returns>
    /// <exception cref="InvalidOperationException"> Thrown if no conveyor for given <typeparamref name="TData"/> and <typeparamref name="TResult"/> is registered </exception>
    /// <seealso cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData, int, CancellationToken, int)"/>
    public static Task<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, TData data, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
    {
        var service = provider.GetService(typeof(IPriorityConveyor<TData, TResult>)) ?? provider.GetService(typeof(IConveyor<TData, TResult>));
        return service switch
        {
            IPriorityConveyor<TData, TResult> priorityConveyor => priorityConveyor.ProcessDataAsync(data, priority, cancellation, attemptsCount),
            IConveyor<TData, TResult> conveyor => conveyor.ProcessDataAsync(data, cancellation, attemptsCount),
            _ => throw new InvalidOperationException($"No Conveyor service for {typeof(TData)} -> {typeof(TResult)} found")
        };
    }
}

}
