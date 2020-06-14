/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background
{

public static class ConveyorHelper
{
    public static Task<TResult> ProcessData<TData, TResult>(this IServiceProvider provider, TData data, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
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
