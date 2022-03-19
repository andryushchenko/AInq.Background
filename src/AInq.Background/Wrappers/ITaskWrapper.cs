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

namespace AInq.Background.Wrappers;

/// <summary> Interface for background task wrappers </summary>
/// <typeparam name="TArgument"> Task argument type </typeparam>
public interface ITaskWrapper<in TArgument>
{
    /// <summary> Check if task is cancelled </summary>
    [PublicAPI]
    bool IsCanceled { get; }

    /// <summary> Check if task is completed </summary>
    [PublicAPI]
    bool IsCompleted { get; }

    /// <summary> Check if task is faulted </summary>
    [PublicAPI]
    bool IsFaulted { get; }

    /// <summary> Execute task asynchronously </summary>
    /// <param name="argument"> Task argument </param>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="logger"> Logger instance </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <returns> If task is completed or should be reverted to queue/conveyor </returns>
    [PublicAPI]
    Task<bool> ExecuteAsync(TArgument argument, IServiceProvider provider, ILogger logger, CancellationToken cancellation = default);
}
