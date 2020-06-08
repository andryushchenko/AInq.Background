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

using System;
using System.Threading;

namespace AInq.Background.Managers
{

internal sealed class WorkSchedulerManager : IWorkScheduler
{
    void IWorkScheduler.AddDelayedWork(IWork work, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedWork(Func<IServiceProvider, IWork> workFactory, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncWork(Func<IServiceProvider, IAsyncWork> workFactory, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedWork<TWork>(TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedWork<TResult, TWork>(TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TAsyncWork>(TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncWork<TResult, TAsyncWork>(TimeSpan delay, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedQueueWork(IWork work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedQueueWork<TResult>(IWork<TResult> work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork(IAsyncWork work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedQueueWork(Func<IServiceProvider, IWork> workFactory, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedQueueWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork(Func<IServiceProvider, IAsyncWork> workFactory, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedQueueWork<TWork>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedQueueWork<TResult, TWork>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TAsyncWork>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddDelayedAsyncQueueWork<TResult, TAsyncWork>(TimeSpan delay, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledWork(Func<IServiceProvider, IWork> workFactory, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncWork(Func<IServiceProvider, IAsyncWork> workFactory, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledWork<TWork>(DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledWork<TResult, TWork>(DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TAsyncWork>(DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncWork<TResult, TAsyncWork>(DateTime time, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledQueueWork(IWork work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledQueueWork<TResult>(IWork<TResult> work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork(IAsyncWork work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TResult>(IAsyncWork<TResult> work, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledQueueWork(Func<IServiceProvider, IWork> workFactory, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledQueueWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork(Func<IServiceProvider, IAsyncWork> workFactory, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledQueueWork<TWork>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledQueueWork<TResult, TWork>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TAsyncWork>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddScheduledAsyncQueueWork<TResult, TAsyncWork>(DateTime time, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronWork(IWork work, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronWork(Func<IServiceProvider, IWork> workFactory, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncWork(Func<IServiceProvider, IAsyncWork> workFactory, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronWork<TWork>(string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronWork<TResult, TWork>(string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncWork<TAsyncWork>(string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncWork<TResult, TAsyncWork>(string cronExpression, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronQueueWork(IWork work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronQueueWork<TResult>(IWork<TResult> work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncQueueWork(IAsyncWork work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TResult>(IAsyncWork<TResult> work, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronQueueWork(Func<IServiceProvider, IWork> workFactory, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronQueueWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncQueueWork(Func<IServiceProvider, IAsyncWork> workFactory, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronQueueWork<TWork>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronQueueWork<TResult, TWork>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TAsyncWork>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    void IWorkScheduler.AddCronAsyncQueueWork<TResult, TAsyncWork>(string cronExpression, int maxAttempt, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}

}
