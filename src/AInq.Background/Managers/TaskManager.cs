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
using Nito.AsyncEx;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Managers
{

internal class TaskManager<TTaskArgument> : ITaskManager<TTaskArgument, object?>
{
    private readonly AsyncAutoResetEvent _newDataEvent = new AsyncAutoResetEvent(false);
    private readonly ConcurrentQueue<ITaskWrapper<TTaskArgument>> _queue = new ConcurrentQueue<ITaskWrapper<TTaskArgument>>();

    bool ITaskManager<TTaskArgument, object?>.HasTask => !_queue.IsEmpty;

    Task ITaskManager<TTaskArgument, object?>.WaitForTaskAsync(CancellationToken cancellation)
        => _queue.IsEmpty
            ? _newDataEvent.WaitAsync(cancellation)
            : Task.CompletedTask;

    (ITaskWrapper<TTaskArgument>?, object?) ITaskManager<TTaskArgument, object?>.GetTask()
    {
        while (true)
        {
            if (!_queue.TryDequeue(out var task))
                return (null, null);
            if (!task.IsCanceled)
                return (task, null);
        }
    }

    void ITaskManager<TTaskArgument, object?>.RevertTask(ITaskWrapper<TTaskArgument> task, object? metadata)
        => _queue.Enqueue(task);

    protected void AddTask(ITaskWrapper<TTaskArgument> task)
    {
        _queue.Enqueue(task);
        _newDataEvent.Set();
    }
}

}
