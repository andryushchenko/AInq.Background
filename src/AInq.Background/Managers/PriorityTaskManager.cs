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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Background.Managers
{

internal class PriorityTaskManager<TTaskArgument> : ITaskManager<TTaskArgument, int>
{
    private readonly AsyncAutoResetEvent _newDataEvent = new AsyncAutoResetEvent(false);
    private readonly IList<ConcurrentQueue<ITaskWrapper<TTaskArgument>>> _queues;

    protected PriorityTaskManager(int maxPriority = 100)
    {
        MaxPriority = Math.Min(100, Math.Max(1, maxPriority));
        _queues = new ConcurrentQueue<ITaskWrapper<TTaskArgument>>[MaxPriority + 1];
        for (var index = 0; index <= MaxPriority; index++)
            _queues[index] = new ConcurrentQueue<ITaskWrapper<TTaskArgument>>();
    }

    protected int MaxPriority { get; }

    bool ITaskManager<TTaskArgument, int>.HasTask => _queues.Any(queue => !queue.IsEmpty);

    Task ITaskManager<TTaskArgument, int>.WaitForTaskAsync(CancellationToken cancellation)
        => _queues.Any(queue => !queue.IsEmpty)
            ? Task.CompletedTask
            : _newDataEvent.WaitAsync(cancellation);

    (ITaskWrapper<TTaskArgument>?, int) ITaskManager<TTaskArgument, int>.GetTask()
    {
        while (true)
        {
            var pendingQueue = _queues.FirstOrDefault(queue => !queue.IsEmpty);
            if (pendingQueue == null || !pendingQueue.TryDequeue(out var task))
                return (null, -1);
            if (!task.IsCanceled)
                return (task, _queues.IndexOf(pendingQueue));
        }
    }

    void ITaskManager<TTaskArgument, int>.RevertTask(ITaskWrapper<TTaskArgument> task, int metadata)
    {
        _queues[FixPriority(metadata)].Enqueue(task);
        _newDataEvent.Set();
    }

    protected void AddTask(ITaskWrapper<TTaskArgument> task, int priority)
    {
        _queues[FixPriority(priority)].Enqueue(task);
        _newDataEvent.Set();
    }

    private int FixPriority(int priority)
        => Math.Min(MaxPriority, Math.Max(0, priority));
}

}
