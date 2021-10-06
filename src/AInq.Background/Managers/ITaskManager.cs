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

using AInq.Background.Wrappers;

namespace AInq.Background.Managers;

/// <summary> Interface for background task manager </summary>
/// <typeparam name="TArgument"> Task argument type </typeparam>
/// <typeparam name="TMetadata"> Task metadata type </typeparam>
public interface ITaskManager<TArgument, TMetadata>
{
    /// <summary> Check if manager has pending tasks </summary>
    bool HasTask { get; }

    /// <summary> Asynchronously wait wor pending tasks </summary>
    /// <param name="cancellation"> Wait cancellation token </param>
    Task WaitForTaskAsync(CancellationToken cancellation = default);

    /// <summary> Get first pending task </summary>
    /// <returns> Task wrapper and task metadata </returns>
    (ITaskWrapper<TArgument>?, TMetadata) GetTask();

    /// <summary> Revert uncompleted task to manager </summary>
    /// <param name="task"> Task instance </param>
    /// <param name="metadata"> Task metadata </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="task" /> or <paramref name="metadata" /> is NULL </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="metadata" /> has incorrect value </exception>
    void RevertTask(ITaskWrapper<TArgument> task, TMetadata metadata);
}
