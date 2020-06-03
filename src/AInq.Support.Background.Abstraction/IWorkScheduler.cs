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
    public interface IWorkScheduler
    {
        void AddDelayedWork(IWork work, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedWork<TResult>(IWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedQueueWork(IWork work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork<TResult>(IWork<TResult> work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork(IAsyncWork work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork(IAsyncWork work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledQueueWork(IWork work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork<TResult>(IWork<TResult> work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork(IAsyncWork work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork<TResult>(IAsyncWork<TResult> work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronWork(IWork work, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork(IAsyncWork work, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default);
        void AddCronQueueWork(IWork work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork<TResult>(IWork<TResult> work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork(IAsyncWork work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork<TResult>(IAsyncWork<TResult> work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
    }
}