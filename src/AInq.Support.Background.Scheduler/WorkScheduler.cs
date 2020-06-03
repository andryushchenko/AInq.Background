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

namespace AInq.Support.Background
{
    internal sealed class WorkScheduler: IWorkScheduler
    {
        void IWorkScheduler.AddDelayedWork(IWork work, TimeSpan delay, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddDelayedWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddDelayedWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddDelayedWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation)
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

        void IWorkScheduler.AddDelayedQueueWork(IAsyncWork work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddDelayedQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, int maxAttempt, CancellationToken cancellation)
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

        void IWorkScheduler.AddScheduledWork(IAsyncWork work, DateTime time, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddScheduledWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation)
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

        void IWorkScheduler.AddScheduledQueueWork(IAsyncWork work, DateTime time, int maxAttempt, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddScheduledQueueWork<TResult>(IAsyncWork<TResult> work, DateTime time, int maxAttempt, CancellationToken cancellation)
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

        void IWorkScheduler.AddCronWork(IAsyncWork work, string cronExpression, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddCronWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation)
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

        void IWorkScheduler.AddCronQueueWork(IAsyncWork work, string cronExpression, int maxAttempt, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        void IWorkScheduler.AddCronQueueWork<TResult>(IAsyncWork<TResult> work, string cronExpression, int maxAttempt, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}