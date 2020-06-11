/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using AInq.Background.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AInq.Background.WorkFactory;
using static AInq.Background.Wrappers.CronWorkWrapperFactory;
using static AInq.Background.Wrappers.DelayedWorkWrapperFactory;

namespace AInq.Background.Managers
{

internal sealed class WorkSchedulerManager : IWorkScheduler, IWorkSchedulerManager
{
    private ConcurrentBag<IScheduledTaskWrapper> _works = new ConcurrentBag<IScheduledTaskWrapper>();
    private readonly AsyncAutoResetEvent _newWorkEvent = new AsyncAutoResetEvent(false);

    Task IWorkSchedulerManager.WaitForNewTaskAsync(CancellationToken cancellation)
        => _newWorkEvent.WaitAsync(cancellation);

    public DateTime? GetNextTaskTime()
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

    public ILookup<DateTime, IScheduledTaskWrapper> GetUpcomingTasks(TimeSpan horizon)
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

    void IWorkSchedulerManager.RevertWork(IScheduledTaskWrapper task)
    {
        if (task.IsCanceled || !task.NextScheduledTime.HasValue)
            return;
        _works.Add(task);
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedWork(IWork work, TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedWork<TWork>(TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedWork<TWork, TResult>(TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TAsyncWork>(TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TAsyncWork, TResult>(TimeSpan delay, CancellationToken cancellation)
    {
        _works.Add(CreateDelayedWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedQueueWork(IWork work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedQueueWork<TResult>(IWork<TResult> work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork(IAsyncWork work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedQueueWork<TWork>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork>(cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedQueueWork<TWork, TResult>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork, TResult>(cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TAsyncWork>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork>(cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TAsyncWork, TResult>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, maxAttempt).Ignore()),
            delay <= TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(delay), delay, "Must be greater then 00:00:00.000")
                : delay,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledWork<TWork>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledWork<TWork, TResult>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TAsyncWork>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TAsyncWork, TResult>(DateTime time, CancellationToken cancellation)
    {
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledQueueWork(IWork work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledQueueWork<TResult>(IWork<TResult> work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork(IAsyncWork work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TResult>(IAsyncWork<TResult> work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledQueueWork<TWork>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork>(cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledQueueWork<TWork, TResult>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork, TResult>(cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TAsyncWork>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork>(cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TAsyncWork, TResult>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        time = time.ToLocalTime();
        _works.Add(CreateDelayedWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, maxAttempt).Ignore()),
            time <= DateTime.Now
                ? throw new ArgumentOutOfRangeException(nameof(time), time, "Must be greater then current time")
                : time,
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronWork(IWork work, string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(work ?? throw new ArgumentNullException(nameof(work)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronWork<TWork>(string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronWork<TWork, TResult>(string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<TWork>().DoWork(provider)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncWork<TAsyncWork>(string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncWork<TAsyncWork, TResult>(string cronExpression, CancellationToken cancellation)
    {
        _works.Add(CreateCronWorkWrapper(CreateWork((provider, token) => provider.GetRequiredService<TAsyncWork>().DoWorkAsync(provider, token)),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronQueueWork(IWork work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronQueueWork<TResult>(IWork<TResult> work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork(work, cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncQueueWork(IAsyncWork work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TResult>(IAsyncWork<TResult> work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork(work, cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronQueueWork<TWork>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork>(cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronQueueWork<TWork, TResult>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueWork<TWork, TResult>(cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TAsyncWork>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork>(cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TAsyncWork, TResult>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        if (maxAttempt < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), maxAttempt, "Must be 1 or greater");
        _works.Add(CreateCronWorkWrapper(CreateWork(provider => provider.GetRequiredService<IWorkQueue>().EnqueueAsyncWork<TAsyncWork, TResult>(cancellation, maxAttempt).Ignore()),
            cronExpression?.ParseCron() ?? throw new ArgumentException("Syntax error in cron expression", nameof(cronExpression)),
            cancellation));
        _newWorkEvent.Set();
    }
}

}
