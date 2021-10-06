// Copyright 2020-2021 Anton Andryushchenko
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

/// <summary> Interface for asynchronous access to shared resource of type <typeparamref name="TResource" /> with result of type <typeparamref name="TResult" /> </summary>
/// <typeparam name="TResource"> Shared resource type </typeparam>
/// <typeparam name="TResult"> Access action result type </typeparam>
public interface IAsyncAccess<in TResource, TResult>
    where TResource : notnull
{
    /// <summary> Asynchronous access action </summary>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="serviceProvider"> Service provider instance </param>
    /// <param name="cancellation"> Access action cancellation token </param>
    /// <returns> Access action result task </returns>
    Task<TResult> AccessAsync(TResource resource, IServiceProvider serviceProvider, CancellationToken cancellation = default);
}
