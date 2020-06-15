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

using AInq.Background.Wrappers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Managers
{

internal interface IWorkSchedulerManager
{
    Task WaitForNewTaskAsync(CancellationToken cancellation);
    DateTime? GetNextTaskTime();
    ILookup<DateTime, IScheduledTaskWrapper> GetUpcomingTasks(TimeSpan horizon);
    void RevertWork(IScheduledTaskWrapper task);
}

}
