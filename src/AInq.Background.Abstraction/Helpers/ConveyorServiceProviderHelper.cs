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

using AInq.Background.Services;

namespace AInq.Background.Helpers;

/// <summary> Helper class for <see cref="IConveyor{TData,TResult}" /> and <see cref="IPriorityConveyor{TData,TResult}" /> </summary>
/// <remarks> <see cref="IConveyor{TData,TResult}" /> or <see cref="IPriorityConveyor{TData,TResult}" /> should be registered on host </remarks>
public static class ConveyorServiceProviderHelper
{
    /// <inheritdoc cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData,int,int,CancellationToken)" />
    [PublicAPI]
    public static Task<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, TData data, int priority = 0, int attemptsCount = 1,
        CancellationToken cancellation = default)
        where TData : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityConveyor<TData, TResult>>()
           ?.ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), priority, attemptsCount, cancellation)
           ?? provider.RequiredService<IConveyor<TData, TResult>>()
                      .ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), attemptsCount, cancellation);
}
