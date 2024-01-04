// Copyright 2020-2024 Anton Andryushchenko
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

/// <summary> Factory class for creating <see cref="IWork" /> and <see cref="IAsyncWork" /> from delegates </summary>
public static class WorkFactory
{
    /// <summary> Create <see cref="IWork" /> instance from <see cref="Action{IServiceProvider}" /> </summary>
    /// <param name="work"> Work action </param>
    /// <returns> <see cref="IWork" /> instance for given action </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    public static IWork CreateWork(Action<IServiceProvider> work)
        => new Work(work ?? throw new ArgumentNullException(nameof(work)));

    /// <summary> Create <see cref="IWork{TResult}" /> instance from <see cref="Func{IServiceProvider, TResult}" /> </summary>
    /// <param name="work"> Work function </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> <see cref="IWork{TResult}" /> instance for given function </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    public static IWork<TResult> CreateWork<TResult>(Func<IServiceProvider, TResult> work)
        => new Work<TResult>(work ?? throw new ArgumentNullException(nameof(work)));

    /// <summary> Create <see cref="IAsyncWork" /> instance from <see cref="Func{IServiceProvider, CancellationToken, Task}" /> </summary>
    /// <param name="work"> Work action </param>
    /// <returns> <see cref="IAsyncWork" /> instance for given action </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    public static IAsyncWork CreateAsyncWork(Func<IServiceProvider, CancellationToken, Task> work)
        => new AsyncWork(work ?? throw new ArgumentNullException(nameof(work)));

    /// <summary> Create <see cref="IAsyncWork{TResult}" /> instance from <see cref="Func{IServiceProvider, CancellationToken, Task}" /> </summary>
    /// <param name="work"> Work function </param>
    /// <typeparam name="TResult"> Work result type </typeparam>
    /// <returns> <see cref="IAsyncWork{TResult}" /> instance for given function </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="work" /> is NULL </exception>
    [PublicAPI]
    public static IAsyncWork<TResult> CreateAsyncWork<TResult>(Func<IServiceProvider, CancellationToken, Task<TResult>> work)
        => new AsyncWork<TResult>(work ?? throw new ArgumentNullException(nameof(work)));

    private class Work : IWork
    {
        private readonly Action<IServiceProvider> _work;

        internal Work(Action<IServiceProvider> work)
            => _work = work ?? throw new ArgumentNullException(nameof(work));

        void IWork.DoWork(IServiceProvider serviceProvider)
            => _work.Invoke(serviceProvider);
    }

    private class Work<TResult> : IWork<TResult>
    {
        private readonly Func<IServiceProvider, TResult> _work;

        internal Work(Func<IServiceProvider, TResult> work)
            => _work = work ?? throw new ArgumentNullException(nameof(work));

        TResult IWork<TResult>.DoWork(IServiceProvider serviceProvider)
            => _work.Invoke(serviceProvider);
    }

    private class AsyncWork : IAsyncWork
    {
        private readonly Func<IServiceProvider, CancellationToken, Task> _work;

        internal AsyncWork(Func<IServiceProvider, CancellationToken, Task> work)
            => _work = work ?? throw new ArgumentNullException(nameof(work));

        Task IAsyncWork.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => _work.Invoke(serviceProvider, cancellation);
    }

    private class AsyncWork<TResult> : IAsyncWork<TResult>
    {
        private readonly Func<IServiceProvider, CancellationToken, Task<TResult>> _work;

        internal AsyncWork(Func<IServiceProvider, CancellationToken, Task<TResult>> work)
            => _work = work ?? throw new ArgumentNullException(nameof(work));

        Task<TResult> IAsyncWork<TResult>.DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellation)
            => _work.Invoke(serviceProvider, cancellation);
    }
}
