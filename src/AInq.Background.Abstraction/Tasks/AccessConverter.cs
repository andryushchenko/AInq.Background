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

/// <summary> Extension class for converting <see cref="IAccess{TResource}" /> to <see cref="IAsyncAccess{TResource}" /> </summary>
public static class AccessConverter
{
    /// <summary> Create <see cref="IAsyncAccess{TResource}" /> from <see cref="IAccess{TResource}" /> </summary>
    /// <param name="access"> Access instance </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <returns> <see cref="IAsyncAccess{TResource}" /> wrapper instance </returns>
    [PublicAPI]
    public static IAsyncAccess<TResource> AsAsync<TResource>(this IAccess<TResource> access)
        where TResource : notnull
        => new AsyncAccess<TResource>(access ?? throw new NullReferenceException(nameof(access)));

    /// <summary> Create <see cref="IAsyncAccess{TResource, TResult}" /> from <see cref="IAccess{TResource, TResult}" /> </summary>
    /// <param name="access"> Access instance </param>
    /// <typeparam name="TResource"> Shared resource type </typeparam>
    /// <typeparam name="TResult"> Access result type </typeparam>
    /// <returns> <see cref="IAsyncAccess{TResource, TResult}" /> wrapper instance </returns>
    [PublicAPI]
    public static IAsyncAccess<TResource, TResult> AsAsync<TResource, TResult>(this IAccess<TResource, TResult> access)
        where TResource : notnull
        => new AsyncAccess<TResource, TResult>(access ?? throw new NullReferenceException(nameof(access)));

    private class AsyncAccess<TResource>(IAccess<TResource> access) : IAsyncAccess<TResource>
        where TResource : notnull
    {
        private readonly IAccess<TResource> _access = access ?? throw new ArgumentNullException(nameof(access));

        Task IAsyncAccess<TResource>.AccessAsync(TResource resource, IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            try
            {
                _access.Access(resource, serviceProvider);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
    }

    private class AsyncAccess<TResource, TResult>(IAccess<TResource, TResult> access) : IAsyncAccess<TResource, TResult>
        where TResource : notnull
    {
        private readonly IAccess<TResource, TResult> _access = access ?? throw new ArgumentNullException(nameof(access));

        Task<TResult> IAsyncAccess<TResource, TResult>.AccessAsync(TResource resource, IServiceProvider serviceProvider,
            CancellationToken cancellation)
        {
            try
            {
                return Task.FromResult(_access.Access(resource, serviceProvider));
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
    }
}
