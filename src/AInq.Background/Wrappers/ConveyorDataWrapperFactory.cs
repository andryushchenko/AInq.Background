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

namespace AInq.Background.Wrappers;

/// <summary> Factory class for creating <see cref="ITaskWrapper{TArgument}" /> for background data conveyors </summary>
public static class ConveyorDataWrapperFactory
{
    /// <summary> Create <see cref="ITaskWrapper{TArgument}" /> with given conveyor data </summary>
    /// <param name="data"> Conveyor data </param>
    /// <param name="attemptsCount"> Retry on fail attempts count </param>
    /// <param name="cancellation"> Processing cancellation token </param>
    /// <typeparam name="TData"> Conveyor input data type </typeparam>
    /// <typeparam name="TResult"> Processing result type </typeparam>
    /// <returns> Wrapper and processing result task </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="data" /> is NULL </exception>
    [PublicAPI]
    public static (ITaskWrapper<IConveyorMachine<TData, TResult>>, Task<TResult>) CreateConveyorDataWrapper<TData, TResult>(TData data,
        int attemptsCount = 1, CancellationToken cancellation = default)
        where TData : notnull
    {
        var wrapper = new ConveyorDataWrapper<TData, TResult>(data, Math.Max(1, attemptsCount), cancellation);
        return (wrapper, wrapper.Result);
    }

    private class ConveyorDataWrapper<TData, TResult> : ITaskWrapper<IConveyorMachine<TData, TResult>>
        where TData : notnull
    {
        private readonly TaskCompletionSource<TResult> _completion = new();
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
        bool ITaskWrapper<IConveyorMachine<TData, TResult>>.IsCompleted => _completion.Task.IsCompleted;
        bool ITaskWrapper<IConveyorMachine<TData, TResult>>.IsFaulted => _completion.Task.IsFaulted;

        async Task<bool> ITaskWrapper<IConveyorMachine<TData, TResult>>.ExecuteAsync(IConveyorMachine<TData, TResult> argument,
            IServiceProvider provider, ILogger logger, CancellationToken outerCancellation)
        {
            if (_attemptsRemain < 1)
            {
                _completion.TrySetException(new InvalidOperationException("No attempts left"));
                await _cancellationRegistration.DisposeAsync().ConfigureAwait(false);
                _cancellationRegistration = default;
                return true;
            }
            _attemptsRemain--;
            try
            {
                using var aggregateCancellation = CancellationTokenSource.CreateLinkedTokenSource(_innerCancellation, outerCancellation);
                aggregateCancellation.Token.ThrowIfCancellationRequested();
                _completion.TrySetResult(await argument.ProcessDataAsync(_data, provider, aggregateCancellation.Token).ConfigureAwait(false));
            }
            catch (ArgumentException ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Bad data {Data}", _data);
                _completion.TrySetException(ex);
            }
            catch (OperationCanceledException ex)
            {
                if (outerCancellation.IsCancellationRequested && logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning("Processing data {Data} with machine {Machine} canceled by runtime", _data, argument.GetType());
                if (!_innerCancellation.IsCancellationRequested)
                    _attemptsRemain++;
                if (_attemptsRemain > 0 && !_innerCancellation.IsCancellationRequested)
                    return false;
                _completion.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Error processing data {Data} with machine {Machine}", _data, argument.GetType());
                if (_attemptsRemain > 0)
                    return false;
                _completion.TrySetException(ex);
            }
            await _cancellationRegistration.DisposeAsync().ConfigureAwait(false);
            _cancellationRegistration = default;
            return true;
        }
    }
}
