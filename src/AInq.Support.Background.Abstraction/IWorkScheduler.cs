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
        void AddDelayedAsyncWork(IAsyncWork work, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedAsyncWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedWork(Func<IServiceProvider, IWork> workFactory, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedAsyncWork(Func<IServiceProvider, IAsyncWork> workFactory, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedAsyncWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, TimeSpan delay, CancellationToken cancellation = default);
        void AddDelayedWork<TWork>(TimeSpan delay, CancellationToken cancellation = default) where TWork:IWork;
        void AddDelayedWork<TResult, TWork>(TimeSpan delay, CancellationToken cancellation = default) where TWork:IWork<TResult>;
        void AddDelayedAsyncWork<TAsyncWork>(TimeSpan delay, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork;
        void AddDelayedAsyncWork<TResult, TAsyncWork>(TimeSpan delay, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork<TResult>;

        void AddDelayedQueueWork(IWork work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork<TResult>(IWork<TResult> work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedAsyncQueueWork(IAsyncWork work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedAsyncQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork(Func<IServiceProvider, IWork> workFactory, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedAsyncQueueWork(Func<IServiceProvider, IAsyncWork> workFactory, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedAsyncQueueWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddDelayedQueueWork<TWork>(TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default) where TWork:IWork;
        void AddDelayedQueueWork<TResult, TWork>(TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default) where TWork:IWork<TResult>;
        void AddDelayedAsyncQueueWork<TAsyncWork>(TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork;
        void AddDelayedAsyncQueueWork<TResult, TAsyncWork>(TimeSpan delay, int maxAttempt = 1, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork<TResult>;

        void AddScheduledWork(IWork work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork<TResult>(IWork<TResult> work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledAsyncWork(IAsyncWork work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledAsyncWork<TResult>(IAsyncWork<TResult> work, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork(Func<IServiceProvider, IWork> workFactory, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, DateTime time, CancellationToken cancellation = default);
        void AddScheduledAsyncWork(Func<IServiceProvider, IAsyncWork> workFactory, DateTime time, CancellationToken cancellation = default);
        void AddScheduledAsyncWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, DateTime time, CancellationToken cancellation = default);
        void AddScheduledWork<TWork>(DateTime time, CancellationToken cancellation = default) where TWork:IWork;
        void AddScheduledWork<TResult, TWork>(DateTime time, CancellationToken cancellation = default) where TWork:IWork<TResult>;
        void AddScheduledAsyncWork<TAsyncWork>(DateTime time, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork;
        void AddScheduledAsyncWork<TResult, TAsyncWork>(DateTime time, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork<TResult>;

        void AddScheduledQueueWork(IWork work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork<TResult>(IWork<TResult> work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledAsyncQueueWork(IAsyncWork work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledAsyncQueueWork<TResult>(IAsyncWork<TResult> work, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork(Func<IServiceProvider, IWork> workFactory, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledAsyncQueueWork(Func<IServiceProvider, IAsyncWork> workFactory, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledAsyncQueueWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, DateTime time, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddScheduledQueueWork<TWork>(DateTime time, int maxAttempt = 1, CancellationToken cancellation = default) where TWork:IWork;
        void AddScheduledQueueWork<TResult, TWork>(DateTime time, int maxAttempt = 1, CancellationToken cancellation = default) where TWork:IWork<TResult>;
        void AddScheduledAsyncQueueWork<TAsyncWork>(DateTime time, int maxAttempt = 1, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork;
        void AddScheduledAsyncQueueWork<TResult, TAsyncWork>(DateTime time, int maxAttempt = 1, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork<TResult>;

        void AddCronWork(IWork work, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork<TResult>(IWork<TResult> work, string cronExpression, CancellationToken cancellation = default);
        void AddCronAsyncWork(IAsyncWork work, string cronExpression, CancellationToken cancellation = default);
        void AddCronAsyncWork<TResult>(IAsyncWork<TResult> work, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork(Func<IServiceProvider, IWork> workFactory, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, string cronExpression, CancellationToken cancellation = default);
        void AddCronAsyncWork(Func<IServiceProvider, IAsyncWork> workFactory, string cronExpression, CancellationToken cancellation = default);
        void AddCronAsyncWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, string cronExpression, CancellationToken cancellation = default);
        void AddCronWork<TWork>(string cronExpression, CancellationToken cancellation = default) where TWork:IWork;
        void AddCronWork<TResult, TWork>(string cronExpression, CancellationToken cancellation = default) where TWork:IWork<TResult>;
        void AddCronAsyncWork<TAsyncWork>(string cronExpression, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork;
        void AddCronAsyncWork<TResult, TAsyncWork>(string cronExpression, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork<TResult>;

        void AddCronQueueWork(IWork work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork<TResult>(IWork<TResult> work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronAsyncQueueWork(IAsyncWork work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronAsyncQueueWork<TResult>(IAsyncWork<TResult> work, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork(Func<IServiceProvider, IWork> workFactory, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork<TResult>(Func<IServiceProvider, IWork<TResult>> workFactory, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronAsyncQueueWork(Func<IServiceProvider, IAsyncWork> workFactory, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronAsyncQueueWork<TResult>(Func<IServiceProvider, IAsyncWork<TResult>> workFactory, string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default);
        void AddCronQueueWork<TWork>(string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default) where TWork:IWork;
        void AddCronQueueWork<TResult, TWork>(string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default) where TWork:IWork<TResult>;
        void AddCronAsyncQueueWork<TAsyncWork>(string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork;
        void AddCronAsyncQueueWork<TResult, TAsyncWork>(string cronExpression, int maxAttempt = 1, CancellationToken cancellation = default) where TAsyncWork:IAsyncWork<TResult>;
    }
}