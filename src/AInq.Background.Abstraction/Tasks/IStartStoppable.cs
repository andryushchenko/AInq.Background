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

using AInq.Background.Services;

namespace AInq.Background.Tasks;

/// <summary> Interface for object which need to be activated/deactivated before/after usage </summary>
/// <remarks> Used in <see cref="IConveyor{TData,TResult}" /> and <see cref="IAccessQueue{TResource}" /></remarks>
public interface IStartStoppable
{
    /// <summary> Shows if object is active now </summary>
    bool IsActive { get; }

    /// <summary> Activate object asynchronously </summary>
    /// <param name="cancellation"> Activation cancellation token </param>
    /// <returns> Activation completion task </returns>
    Task ActivateAsync(CancellationToken cancellation = default);

    /// <summary> Deactivate object asynchronously  </summary>
    /// <param name="cancellation"> Deactivation cancellation token </param>
    /// <returns> Deactivation completion task </returns>
    Task DeactivateAsync(CancellationToken cancellation = default);
}
