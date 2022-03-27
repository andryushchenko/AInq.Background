// Copyright 2020-2022 Anton Andryushchenko
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
    /// <inheritdoc cref="IPriorityConveyor{TData,TResult}.ProcessDataAsync(TData, int, CancellationToken, int)" />
    [PublicAPI]
    public static Task<TResult> ProcessDataAsync<TData, TResult>(this IServiceProvider provider, TData data, CancellationToken cancellation = default,
        int attemptsCount = 1, int priority = 0)
        where TData : notnull
        => (provider ?? throw new ArgumentNullException(nameof(provider)))
           .Service<IPriorityConveyor<TData, TResult>>()
           ?.ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), priority, cancellation, attemptsCount)
           ?? provider.RequiredService<IConveyor<TData, TResult>>()
                      .ProcessDataAsync(data ?? throw new ArgumentNullException(nameof(data)), cancellation, attemptsCount);
}
