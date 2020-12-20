﻿// Copyright 2020 Anton Andryushchenko
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
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.Tasks.WorkFactory;
using static AInq.Background.Wrappers.CronWorkWrapperFactory;
using static AInq.Background.Wrappers.DelayedWorkWrapperFactory;

namespace AInq.Background.Managers
{

/// <summary> Work scheduler manager </summary>
public sealed class WorkSchedulerManager : IWorkScheduler, IWorkSchedulerManager
{
    private readonly AsyncAutoResetEvent _newWorkEvent = new(false);
    private ConcurrentBag<IScheduledTaskWrapper> _works = new();

    Task IWorkScheduler.AddDelayedWork(IWork work, TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddDelayedWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddDelayedAsyncWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddDelayedAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddDelayedWork<TWork>(TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddDelayedWork<TWork, TResult>(TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddDelayedAsyncWork<TAsyncWork>(TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(
            CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddDelayedAsyncWork<TAsyncWork, TResult>(TimeSpan delay, CancellationToken cancellation)
    {
        var (wrapper, task) = CreateDelayedWorkWrapper(
            CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddScheduledWork<TWork>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledWork<TWork, TResult>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task IWorkScheduler.AddScheduledAsyncWork<TAsyncWork>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(
            CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    Task<TResult> IWorkScheduler.AddScheduledAsyncWork<TAsyncWork, TResult>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        var (wrapper, task) = CreateDelayedWorkWrapper(
            CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return task;
    }

    IObservable<bool> IWorkScheduler.AddCronWork(IWork work, string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression.ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<TResult> IWorkScheduler.AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<bool> IWorkScheduler.AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<TResult> IWorkScheduler.AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<bool> IWorkScheduler.AddCronWork<TWork>(string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<TResult> IWorkScheduler.AddCronWork<TWork, TResult>(string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<bool> IWorkScheduler.AddCronAsyncWork<TAsyncWork>(string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(
            CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

    IObservable<TResult> IWorkScheduler.AddCronAsyncWork<TAsyncWork, TResult>(string cronExpression, CancellationToken cancellation)
    {
        var (wrapper, observable) = CreateCronWorkWrapper(
            CreateAsyncWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            (cronExpression ?? throw new ArgumentNullException(nameof(cronExpression))).ParseCron(),
            cancellation);
        _works.Add(wrapper);
        _newWorkEvent.Set();
        return observable;
    }

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
