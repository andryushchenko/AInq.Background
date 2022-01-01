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

using AInq.Background.Helpers;
using AInq.Background.Services;
using AInq.Background.Wrappers;
using Nito.AsyncEx;
using System.Collections.Concurrent;
using static AInq.Background.Wrappers.CronWorkWrapperFactory;
using static AInq.Background.Wrappers.RepeatedWorkWrapperFactory;
using static AInq.Background.Wrappers.ScheduledWorkWrapperFactory;

namespace AInq.Background.Managers;

/// <summary> Work scheduler manager </summary>
public sealed class WorkSchedulerManager : IWorkScheduler, IWorkSchedulerManager
{
    private readonly AsyncAutoResetEvent _newWorkEvent = new(false);
    private ConcurrentBag<IScheduledTaskWrapper> _works = new();

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

#region Scheduled

    Task IWorkScheduler.AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateScheduledWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater than current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateScheduledWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater than current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateScheduledWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater than current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateScheduledWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater than current time") : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

#endregion

#region Cron

    IObservable<Maybe<Exception>> IWorkScheduler.AddCronWork(IWork work, string cronExpression, CancellationToken cancellation, int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression.ParseCron(),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<Try<TResult>> IWorkScheduler.AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation,
        int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<Maybe<Exception>> IWorkScheduler.AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation,
        int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<Try<TResult>> IWorkScheduler.AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression,
        CancellationToken cancellation,
        int execCount)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

#endregion

#region RepeatedScheduled

    IObservable<Maybe<Exception>> IWorkScheduler.AddRepeatedWork(IWork work, DateTime startTime, TimeSpan repeatDelay, CancellationToken cancellation,
        int execCount)
    {
        var (wrapper, observable) = CreateRepeatedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay > TimeSpan.Zero
                ? repeatDelay
                : throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater than 00:00:00.000"),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<Try<TResult>> IWorkScheduler.AddRepeatedWork<TResult>(IWork<TResult> work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation, int execCount)
    {
        var (wrapper, observable) = CreateRepeatedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay > TimeSpan.Zero
                ? repeatDelay
                : throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater than 00:00:00.000"),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<Maybe<Exception>> IWorkScheduler.AddRepeatedAsyncWork(IAsyncWork work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation, int execCount)
    {
        var (wrapper, observable) = CreateRepeatedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay > TimeSpan.Zero
                ? repeatDelay
                : throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater than 00:00:00.000"),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<Try<TResult>> IWorkScheduler.AddRepeatedAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime startTime, TimeSpan repeatDelay,
        CancellationToken cancellation, int execCount)
    {
        var (wrapper, observable) = CreateRepeatedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            startTime,
            repeatDelay > TimeSpan.Zero
                ? repeatDelay
                : throw new ArgumentOutOfRangeException(nameof(repeatDelay), repeatDelay, "Must be greater than 00:00:00.000"),
            cancellation,
            execCount is > 0 or -1
                ? execCount
                : throw new ArgumentOutOfRangeException(nameof(execCount), execCount, "Must be greater than 0 or -1 for unlimited repeat"));
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

#endregion
}
