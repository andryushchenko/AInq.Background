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

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Elements
{
    internal sealed class ConveyorElement<TData, TResult> : ITaskWrapper<IConveyorMachine<TData, TResult>>
    {
        private readonly TData _data;
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;

        internal Task<TResult> Result => _completion.Task;

        internal ConveyorElement(TData data, CancellationToken innerCancellation, int attemptsCount)
        {
            if (attemptsCount < 1)
                throw new ArgumentOutOfRangeException(nameof(attemptsCount), attemptsCount, null);
            _data = data;
            _innerCancellation = innerCancellation;
            _attemptsRemain = attemptsCount;
        }

        async Task<bool> ITaskWrapper<IConveyorMachine<TData, TResult>>.ExecuteAsync(IConveyorMachine<TData, TResult> argument, IServiceProvider provider, ILogger logger, CancellationToken cancellation)
        {
            if (_attemptsRemain < 1)
                return true;
            if (argument == null)
                return false;
            _attemptsRemain--;
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, cancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(await argument.ProcessDataAsync(_data, provider, aggregateCancellation.Token));
            }
            catch (ArgumentException ex)
            {
                logger?.LogError(ex, "Bad data {0}", _data);
                _completion.TrySetException(ex);
            }
            catch (OperationCanceledException ex)
            {
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error processing data {0}", _data);
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            return true;
        }
    }
}