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

using AInq.Background.Helpers;
using AInq.Background.Services;
using AInq.Background.Tasks;
using AInq.Background.Wrappers;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Wrappers.CronWorkWrapperFactory;
using static AInq.Background.Wrappers.DelayedWorkWrapperFactory;

namespace AInq.Background.Managers
{

/// <summary> Work scheduler manager </summary>
public sealed class WorkSchedulerManager : IWorkScheduler, IWorkSchedulerManager
{
    private readonly AsyncAutoResetEvent _newWorkEvent = new(false);
    private ConcurrentBag<IScheduledTaskWrapper> _works = new();

    Task IWorkScheduler.AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    IObservable<object?> IWorkScheduler.AddCronWork(IWork work, string cronExpression, CancellationToken cancellation, int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression.ParseCron(),
            cancellation,
            execCount);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<TResult> IWorkScheduler.AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation,
        int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation,
            execCount);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<object?> IWorkScheduler.AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation, int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation,
            execCount);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<TResult> IWorkScheduler.AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation,
        int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation,
            execCount);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<object?> IWorkScheduler.AddRepeatedWork(IWork work, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation,
        int execCount)
        => throw new NotImplementedException();

    IObservable<TResult> IWorkScheduler.AddRepeatedWork<TResult>(IWork<TResult> work, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation, int execCount)
        => throw new NotImplementedException();

    IObservable<object?> IWorkScheduler.AddRepeatedAsyncWork(IAsyncWork work, DateTime starTime, TimeSpan repeatDelay, CancellationToken cancellation,
        int execCount)
        => throw new NotImplementedException();

    IObservable<TResult> IWorkScheduler.AddRepeatedAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime starTime, TimeSpan repeatDelay,
        CancellationToken cancellation, int execCount)
        => throw new NotImplementedException();

    Task IWorkSchedulerManager.WaitForNewTaskAsync(CancellationToken cancellation)
        => _newWorkEvent.WaitAsync(cancellation);

    DateTime? IWorkSchedulerManager.GetNextTaskTime()
    {
        var works = Interlocked.Exchange(ref _works, new ConcurrentBag<IScheduledTaskWrapper>());
        DateTime? next = null;
        while (!works.IsEmpty)
            if (works.TryTake(out var work) && !work.IsCanceled && work.NextScheduledTime.HasValue)
            {
                if (next == null || work.NextScheduledTime!.Value < next.Value)
                    next = work.NextScheduledTime;
                _works.Add(work);
            }
        return next;
    }

    ILookup<DateTime, IScheduledTaskWrapper> IWorkSchedulerManager.GetUpcomingTasks(TimeSpan horizon)
    {
        var works = Interlocked.Exchange(ref _works, new ConcurrentBag<IScheduledTaskWrapper>());
        var upcoming = new LinkedList<IScheduledTaskWrapper>();
        var time = DateTime.Now.Add(horizon);
        while (!works.IsEmpty)
            if (works.TryTake(out var work) && !work.IsCanceled && work.NextScheduledTime.HasValue)
            {
                if (work.NextScheduledTime!.Value <= time)
                    upcoming.AddLast(work);
                else _works.Add(work);
            }
        return upcoming.ToLookup(work => work.NextScheduledTime ?? DateTime.Now);
    }

    void IWorkSchedulerManager.RevertTask(IScheduledTaskWrapper task)
    {
        if (task.IsCanceled || !task.NextScheduledTime.HasValue)
            return;
        _works.Add(task);
        _newWorkEvent.Set();
    }
}

}
