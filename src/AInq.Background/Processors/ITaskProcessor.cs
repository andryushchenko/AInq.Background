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

using AInq.Background.Managers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Processors
{

/// <summary> Interface for task processor </summary>
/// <typeparam name="TArgument"> Task argument type </typeparam>
/// <typeparam name="TMetadata"> Task metadata type </typeparam>
public interface ITaskProcessor<TArgument, TMetadata>
{
    /// <summary> Process pending tasks form given <paramref name="manager" /> </summary>
    /// <param name="manager"> Task manager instance </param>
    /// <param name="provider"> Service provider instance </param>
    /// <param name="logger"> Logger instance </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <returns></returns>
    Task ProcessPendingTasksAsync(ITaskManager<TArgument, TMetadata> manager, IServiceProvider provider, ILogger? logger = null,
        CancellationToken cancellation = default);
}

}
