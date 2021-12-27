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

/// <summary> Basic task manager with single queue </summary>
/// <typeparam name="TArgument"> Task argument type </typeparam>
public class TaskManager<TArgument> : ITaskManager<TArgument, object?>
{
    private readonly AsyncAutoResetEvent _newDataEvent = new(false);
    private readonly ConcurrentQueue<ITaskWrapper<TArgument>> _queue = new();

    bool ITaskManager<TArgument, object?>.HasTask => !_queue.IsEmpty;

    Task ITaskManager<TArgument, object?>.WaitForTaskAsync(CancellationToken cancellation)
        => _queue.IsEmpty ? _newDataEvent.WaitAsync(cancellation) : Task.CompletedTask;

    (ITaskWrapper<TArgument>?, object?) ITaskManager<TArgument, object?>.GetTask()
    {
        while (true)
        {
            if (!_queue.TryDequeue(out var task)) return (null, null);
            if (!task.IsCanceled) return (task, null);
        }
    }

    void ITaskManager<TArgument, object?>.RevertTask(ITaskWrapper<TArgument> task, object? metadata)
        => AddTask(task);

    /// <summary> Add task to queue </summary>
    /// <param name="task"> Task instance </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="task" /> is NULL </exception>
    protected void AddTask(ITaskWrapper<TArgument> task)
    {
        if (task.IsCanceled || task.IsCompleted || task.IsFaulted) return;
        _queue.Enqueue(task ?? throw new ArgumentNullException(nameof(task)));
        _newDataEvent.Set();
    }
}
