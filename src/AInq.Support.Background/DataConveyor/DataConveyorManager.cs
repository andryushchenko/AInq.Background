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

using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.DataConveyor
{
    internal class DataConveyorManager<TData, TResult> : IDataConveyor<TData, TResult>
    {
        protected internal AsyncAutoResetEvent NewDataEvent { get; } = new AsyncAutoResetEvent(false);
        protected internal ConcurrentQueue<DataConveyorElement<TData, TResult>> Queue { get; } = new ConcurrentQueue<DataConveyorElement<TData, TResult>>();

        Task<TResult> IDataConveyor<TData, TResult>.ProcessDataAsync(TData data, CancellationToken cancellation, int attemptsCount)
        {
            if (attemptsCount <= 0) throw new ArgumentOutOfRangeException(nameof(attemptsCount));
            var element = new DataConveyorElement<TData, TResult>(data, cancellation, attemptsCount);
            Queue.Enqueue(element);
            NewDataEvent.Set();
            return element.Result;
        }
    }
}