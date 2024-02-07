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

namespace AInq.Background.Tasks;

/// <summary> Extension class for converting <see cref="IWork" /> to <see cref="IAsyncWork" /> </summary>
public static class WorkConverter
{
    /// <summary> Create <see cref="IAsyncWork" /> from <see cref="IWork" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <returns> <see cref="IAsyncWork{TResult}" /> wrapper instance </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    public static IAsyncWork AsAsync(this IWork work)
        => new AsyncWork(work ?? throw new NullReferenceException(nameof(work)));

    /// <summary> Create <see cref="IAsyncWork{TResult}" /> from <see cref="IWork{TResult}" /> </summary>
    /// <param name="work"> Work instance </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> <see cref="IAsyncWork{TResult}" /> wrapper instance </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    public static IAsyncWork<TResult> AsAsync<TResult>(this IWork<TResult> work)
        => new AsyncWork<TResult>(work ?? throw new NullReferenceException(nameof(work)));

    private class AsyncWork : IAsyncWork
    {
        private readonly IWork _work;

        internal AsyncWork(IWork work)
            => _work = work ?? throw new ArgumentNullException(nameof(work));

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            try
            {
                _work.DoWork(serviceProvider);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
    }

    private class AsyncWork<TResult> : IAsyncWork<TResult>
    {
        private readonly IWork<TResult> _work;

        internal AsyncWork(IWork<TResult> work)
            => _work = work ?? throw new ArgumentNullException(nameof(work));

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            try
            {
                return Task.FromResult(_work.DoWork(serviceProvider));
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
    }
}
