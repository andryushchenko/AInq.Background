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

/// <summary> Basic task manager with numeric prioritization </summary>
/// <typeparam name="TArgument"> Task argument type </typeparam>
public class PriorityTaskManager<TArgument> : ITaskManager<TArgument, int>
{
    private readonly AsyncAutoResetEvent _newDataEvent = new(false);
    private readonly IList<ConcurrentQueue<ITaskWrapper<TArgument>>> _queues;

    /// <param name="maxPriority"> Max allowed priority </param>
    protected PriorityTaskManager(int maxPriority = 100)
    {
        MaxPriority = Math.Min(100, Math.Max(1, maxPriority));
        _queues = new ConcurrentQueue<ITaskWrapper<TArgument>>[MaxPriority + 1];
        for (var index = 0; index <= MaxPriority; index++)
            _queues[index] = new ConcurrentQueue<ITaskWrapper<TArgument>>();
    }

    /// <summary> Max allowed priority </summary>
    protected int MaxPriority { get; }

    bool ITaskManager<TArgument, int>.HasTask => _queues.Any(queue => !queue.IsEmpty);

    Task ITaskManager<TArgument, int>.WaitForTaskAsync(CancellationToken cancellation)
        => _queues.Any(queue => !queue.IsEmpty) ? Task.CompletedTask : _newDataEvent.WaitAsync(cancellation);

    (ITaskWrapper<TArgument>?, int) ITaskManager<TArgument, int>.GetTask()
    {
        while (true)
        {
            var pendingQueue = _queues.LastOrDefault(queue => !queue.IsEmpty);
            if (pendingQueue == null || !pendingQueue.TryDequeue(out var task))
                return (null, -1);
            if (!task.IsCanceled)
                return (task, _queues.IndexOf(pendingQueue));
        }
    }

    void ITaskManager<TArgument, int>.RevertTask(ITaskWrapper<TArgument> task, int metadata)
        => AddTask(task, metadata);

    /// <summary> Add task to queue </summary>
    /// <param name="task"> Task instance </param>
    /// <param name="priority"> Task priority </param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="task" /> is NULL </exception>
    protected void AddTask(ITaskWrapper<TArgument> task, int priority)
    {
        if (task.IsCanceled || task.IsCompleted || task.IsFaulted)
            return;
        _queues[FixPriority(priority)].Enqueue(task ?? throw new ArgumentNullException(nameof(task)));
        _newDataEvent.Set();
    }

    private int FixPriority(int priority)
        => Math.Min(MaxPriority, Math.Max(0, priority));
}
