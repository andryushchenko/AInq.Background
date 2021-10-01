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

/// <summary> Interface for synchronous access to shared resource of type <typeparamref name="TResource" /> without result </summary>
/// <typeparam name="TResource"> Shared resource type </typeparam>
public interface IAccess<in TResource>
    where TResource : notnull
{
    /// <summary> Access action </summary>
    /// <param name="resource"> Shared resource instance </param>
    /// <param name="serviceProvider"> Service provider instance </param>
    void Access(TResource resource, IServiceProvider serviceProvider);
}
