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

using AInq.Background.Wrappers;

namespace AInq.Background.Managers;

/// <summary> Interface for background work scheduler manager </summary>
public interface IWorkSchedulerManager
{
    /// <summary> Asynchronously wait for new scheduled task </summary>
    /// <param name="cancellation"> Wait cancellation token </param>
    [PublicAPI]
    Task WaitForNewTaskAsync(CancellationToken cancellation);

    /// <summary> Get next scheduled task execution time </summary>
    /// <returns> Time or NULL if no tasks </returns>
    [PublicAPI]
    DateTime? GetNextTaskTime();

    /// <summary> Get upcoming scheduled tasks within given <paramref name="horizon" /> </summary>
    /// <param name="horizon"> Upcoming task search horizon </param>
    /// <returns> Task wrappers collection grouped by time </returns>
    [PublicAPI]
    ILookup<DateTime, IScheduledTaskWrapper> GetUpcomingTasks(TimeSpan horizon);

    /// <summary> Revert task to scheduler </summary>
    /// <param name="task"> Task instance </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="task" /> is NULL </exception>
    [PublicAPI]
    void RevertTask(IScheduledTaskWrapper task);
}
