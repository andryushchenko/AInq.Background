﻿/*
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

namespace AInq.Support.Background.DataConveyor
{
    internal class DataConveyorElement<TData, TResult>
    {
        private readonly TData _data;
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;

        internal Task<TResult> Result => _completion.Task;

        internal DataConveyorElement(TData data, CancellationToken innerCancellation, int attemptsCount)
        {
            if (attemptsCount <= 0) throw new ArgumentOutOfRangeException(nameof(attemptsCount));
            _data = data;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
        }

        internal async Task<bool> ProcessDataAsync(IDataConveyorMachine<TData, TResult> machine, CancellationToken outerCancellation)
        {
            if (_attemptsRemain <= 0) return true;
            _attemptsRemain--;
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(await machine.ProcessDataAsync(_data, aggregateCancellation.Token));
            }
            catch (ArgumentException ex)
            {
                _completion.TrySetException(ex);
            }
            catch (OperationCanceledException ex)
            {
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested) return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                if (_attemptsRemain > 0) return false;
                _completion.TrySetException(ex);
            }
            return true;
        }
    }
}