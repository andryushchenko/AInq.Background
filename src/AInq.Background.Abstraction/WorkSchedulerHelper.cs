/*
 * Copyright 2020 Anton Andryushchenko
 *
 * Licensed under the Apache License, Version 2.0 (this IServiceProvider provider, the "License") => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"));
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

using System;
using System.Threading;

namespace AInq.Background
{

public static class WorkSchedulerHelper
{
    public static void AddDelayedWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork(work, delay, cancellation);

    public static void AddDelayedWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork(work, delay, cancellation);

    public static void AddDelayedAsyncWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork(work, delay, cancellation);

    public static void AddDelayedAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork(work, delay, cancellation);

    public static void AddDelayedWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork<TWork>(delay, cancellation);

    public static void AddDelayedWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedWork<TWork, TResult>(delay, cancellation);

    public static void AddDelayedAsyncWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork<TAsyncWork>(delay, cancellation);

    public static void AddDelayedAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncWork<TAsyncWork, TResult>(delay, cancellation);

    public static void AddDelayedQueueWork(this IServiceProvider provider, IWork work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork(work, delay, cancellation, attemptsCount, priority);

    public static void AddDelayedQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork(work, delay, cancellation, attemptsCount, priority);

    public static void AddDelayedAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork(work, delay, cancellation, attemptsCount, priority);

    public static void AddDelayedAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork(work, delay, cancellation, attemptsCount, priority);

    public static void AddDelayedQueueWork<TWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork<TWork>(delay, cancellation, attemptsCount, priority);

    public static void AddDelayedQueueWork<TWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedQueueWork<TWork, TResult>(delay, cancellation, attemptsCount, priority);

    public static void AddDelayedAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork<TAsyncWork>(delay, cancellation, attemptsCount, priority);

    public static void AddDelayedAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, TimeSpan delay, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddDelayedAsyncQueueWork<TAsyncWork, TResult>(delay, cancellation, attemptsCount, priority);

    public static void AddScheduledWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork(work, time, cancellation);

    public static void AddScheduledWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork(work, time, cancellation);

    public static void AddScheduledAsyncWork(this IServiceProvider provider, IAsyncWork work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork(work, time, cancellation);

    public static void AddScheduledAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork(work, time, cancellation);

    public static void AddScheduledWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork<TWork>(time, cancellation);

    public static void AddScheduledWork<TWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledWork<TWork, TResult>(time, cancellation);

    public static void AddScheduledAsyncWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork<TAsyncWork>(time, cancellation);

    public static void AddScheduledAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncWork<TAsyncWork, TResult>(time, cancellation);

    public static void AddScheduledQueueWork(this IServiceProvider provider, IWork work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    public static void AddScheduledQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork(work, time, cancellation, attemptsCount, priority);

    public static void AddScheduledAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork(work, time, cancellation, attemptsCount, priority);

    public static void AddScheduledAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork(work, time, cancellation, attemptsCount, priority);

    public static void AddScheduledQueueWork<TWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork<TWork>(time, cancellation, attemptsCount, priority);

    public static void AddScheduledQueueWork<TWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledQueueWork<TWork, TResult>(time, cancellation, attemptsCount, priority);

    public static void AddScheduledAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork<TAsyncWork>(time, cancellation, attemptsCount, priority);

    public static void AddScheduledAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, DateTime time, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(time, cancellation, attemptsCount, priority);

    public static void AddCronWork(this IServiceProvider provider, IWork work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork(work, cronExpression, cancellation);

    public static void AddCronWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork(work, cronExpression, cancellation);

    public static void AddCronAsyncWork(this IServiceProvider provider, IAsyncWork work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork(work, cronExpression, cancellation);

    public static void AddCronAsyncWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork(work, cronExpression, cancellation);

    public static void AddCronWork<TWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork<TWork>(cronExpression, cancellation);

    public static void AddCronWork<TWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronWork<TWork, TResult>(cronExpression, cancellation);

    public static void AddCronAsyncWork<TAsyncWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork<TAsyncWork>(cronExpression, cancellation);

    public static void AddCronAsyncWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncWork<TAsyncWork, TResult>(cronExpression, cancellation);

    public static void AddCronQueueWork(this IServiceProvider provider, IWork work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronQueueWork<TResult>(this IServiceProvider provider, IWork<TResult> work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronAsyncQueueWork(this IServiceProvider provider, IAsyncWork work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronAsyncQueueWork<TResult>(this IServiceProvider provider, IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork(work, cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronQueueWork<TWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork<TWork>(cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronQueueWork<TWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TWork : IWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronQueueWork<TWork, TResult>(cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronAsyncQueueWork<TAsyncWork>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork<TAsyncWork>(cronExpression, cancellation, attemptsCount, priority);

    public static void AddCronAsyncQueueWork<TAsyncWork, TResult>(this IServiceProvider provider, string cronExpression, CancellationToken cancellation = default, int attemptsCount = 1, int priority = 0)
        where TAsyncWork : IAsyncWork<TResult>
        => (provider.GetService(typeof(IWorkScheduler)) as IWorkScheduler ?? throw new InvalidOperationException("No Work Scheduler service found"))
            .AddCronAsyncQueueWork<TAsyncWork, TResult>(cronExpression, cancellation, attemptsCount, priority);
}

}
