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

using AInq.Background.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Wrappers
{

internal static class ConveyorDataWrapperFactory
{
    public static (ITaskWrapper<IConveyorMachine<TData, TResult>>, Task<TResult>) CreateConveyorDataWrapper<TData, TResult>(TData data,
        int attemptsCount = 1,
        CancellationToken cancellation = default)
    {
        var wrapper = new ConveyorDataWrapper<TData, TResult>(data, Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.Result);
    }

    private class ConveyorDataWrapper<TData, TResult> : ITaskWrapper<IConveyorMachine<TData, TResult>>
    {
        private readonly TaskCompletionSource<TResult> _completion = new TaskCompletionSource<TResult>();
        private readonly TData _data;
        private readonly CancellationToken _innerCancellation;
        private int _attemptsRemain;
        private CancellationTokenRegistration _cancellationRegistration;

        internal ConveyorDataWrapper(TData data, int attemptsCount, CancellationToken innerCancellation)
        {
            _data = data;
            _innerCancellation = innerCancellation;
            _attemptsRemain = Math.Max(1, attemptsCount);
            _cancellationRegistration = _innerCancellation.Register(() => _completion.TrySetCanceled(_innerCancellation), false);
        }

        internal Task<TResult> Result => _completion.Task;
        bool ITaskWrapper<IConveyorMachine<TData, TResult>>.IsCanceled => _innerCancellation.IsCancellationRequested;

        async Task<bool> ITaskWrapper<IConveyorMachine<TData, TResult>>.ExecuteAsync(IConveyorMachine<TData, TResult> argument,
            IServiceProvider provider,
            ILogger? logger, CancellationToken cancellation)
        {
            if (_attemptsRemain < 1)
            {
                _completion.TrySetException(new InvalidOperationException("No attempts left"));
                _cancellationRegistration.Dispose();
                _cancellationRegistration = default;
                return true;
            }
            if (argument == null)
                return false;
            _attemptsRemain--;
            using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, cancellation);
            try
            {
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(await argument.ProcessDataAsync(_data, provider, aggregateCancellation.Token).ConfigureAwait(false));
            }
            catch (ArgumentException ex)
            {
                logger?.LogError(ex, "Bad data {Data}", _data);
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
                logger?.LogError(ex, "Error processing data {Data} with machine {Type}", _data, argument.GetType());
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            _cancellationRegistration.Dispose();
            _cancellationRegistration = default;
            return true;
        }
    }
}

}