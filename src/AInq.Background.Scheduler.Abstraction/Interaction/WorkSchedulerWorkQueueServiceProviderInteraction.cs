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

namespace AInq.Background.Interaction;

/// <summary> <see cref="IWorkScheduler" /> extensions to run scheduled work in registered background queue </summary>
/// <remarks> <see cref="IWorkScheduler" /> service should be registered on host to schedule work </remarks>
/// <remarks> <see cref="IPriorityWorkQueue" /> or <see cref="IWorkQueue" /> service should be registered on host to run queued work </remarks>
public static class WorkSchedulerWorkQueueServiceProviderInteraction
{
    /// <param name="provider"> Service provider instance </param>
    extension(IServiceProvider provider)
    {
#region DelayedQueue

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork(IWorkScheduler,IWork,TimeSpan,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledQueueWork(IWork work, TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TResult}(IWorkScheduler,IWork{TResult},TimeSpan,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledQueueWork<TResult>(IWork<TResult> work, TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork(IWorkScheduler,IAsyncWork,TimeSpan,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledAsyncQueueWork(IAsyncWork work, TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},TimeSpan,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledAsyncQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

#endregion

#region DelayedQueueDI

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork}(IWorkScheduler,TimeSpan,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledQueueWork<TWork>(TimeSpan delay, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
            where TWork : IWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork<TWork>(delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledQueueWork<TWork, TResult>(TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            where TWork : IWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork<TWork, TResult>(delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler,TimeSpan,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledAsyncQueueWork<TAsyncWork>(TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork<TAsyncWork>(delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(TimeSpan delay, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(delay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

#endregion

#region ScheduledQueue

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork(IWorkScheduler,IWork,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledQueueWork(IWork work, DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TResult}(IWorkScheduler,IWork{TResult},DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledQueueWork<TResult>(IWork<TResult> work, DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork(IWorkScheduler,IAsyncWork,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledAsyncQueueWork(IAsyncWork work, DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledAsyncQueueWork<TResult>(IAsyncWork<TResult> work, DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

#endregion

#region ScheduledQueueDI

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledQueueWork<TWork>(DateTime time, int priority = 0, int attemptsCount = 1, CancellationToken cancellation = default)
            where TWork : IWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork<TWork>(time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledQueueWork{TWork,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledQueueWork<TWork, TResult>(DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            where TWork : IWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledQueueWork<TWork, TResult>(time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task AddScheduledAsyncQueueWork<TAsyncWork>(DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork<TAsyncWork>(time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddScheduledAsyncQueueWork{TAsyncWork,TResult}(IWorkScheduler,DateTime,int,int,CancellationToken)" />
        [PublicAPI]
        public Task<TResult> AddScheduledAsyncQueueWork<TAsyncWork, TResult>(DateTime time, int priority = 0, int attemptsCount = 1,
            CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddScheduledAsyncQueueWork<TAsyncWork, TResult>(time,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  cancellation);

#endregion

#region CronQueue

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddCronQueueWork(IWork work, string cronExpression, int priority = 0, int attemptsCount = 1,
            int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TResult}(IWorkScheduler,IWork{TResult},string,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddCronQueueWork<TResult>(IWork<TResult> work, string cronExpression, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronQueueWork(work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddCronAsyncQueueWork(IAsyncWork work, string cronExpression, int priority = 0, int attemptsCount = 1,
            int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},string,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddCronAsyncQueueWork<TResult>(IAsyncWork<TResult> work, string cronExpression, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

#endregion

#region CronQueueDI

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TWork}(IWorkScheduler,string,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddCronQueueWork<TWork>(string cronExpression, int priority = 0, int attemptsCount = 1,
            int execCount = -1, CancellationToken cancellation = default)
            where TWork : IWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronQueueWork<TWork>(
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronQueueWork{TWork,TResult}" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddCronQueueWork<TWork, TResult>(string cronExpression, int priority = 0, int attemptsCount = 1,
            int execCount = -1, CancellationToken cancellation = default)
            where TWork : IWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronQueueWork<TWork, TResult>(
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TAsyncWork}(IWorkScheduler,string,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddCronAsyncQueueWork<TAsyncWork>(string cronExpression, int priority = 0, int attemptsCount = 1,
            int execCount = -1, CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronAsyncQueueWork<TAsyncWork>(
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddCronAsyncQueueWork{TAsyncWork,TResult}" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddCronAsyncQueueWork<TAsyncWork, TResult>(string cronExpression, int priority = 0, int attemptsCount = 1,
            int execCount = -1, CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddCronAsyncQueueWork<TAsyncWork, TResult>(
                                                                                  cronExpression
                                                                                  ?? throw new ArgumentNullException(nameof(cronExpression)),
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

#endregion

#region RepeatedScheduledQueue

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork(IWorkScheduler,IWork,DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedQueueWork(IWork work, DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TResult}(IWorkScheduler,IWork{TResult},DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(IWork<TResult> work, DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork(IWorkScheduler,IAsyncWork,DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(IAsyncWork work, DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(IAsyncWork<TResult> work, DateTime starTime, TimeSpan repeatDelay,
            int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

#endregion

#region RepeatedScheduledQueueDI

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TWork : IWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork<TWork>(starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork,TResult}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TWork : IWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork<TWork, TResult>(starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork<TAsyncWork>(starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork,TResult}(IWorkScheduler,DateTime,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(DateTime starTime, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(starTime,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

#endregion

#region RepeatedDelayedQueue

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork(IWorkScheduler,IWork,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedQueueWork(IWork work, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TResult}(IWorkScheduler,IWork{TResult},TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedQueueWork<TResult>(IWork<TResult> work, TimeSpan startDelay, TimeSpan repeatDelay,
            int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork(IWorkScheduler,IAsyncWork,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork(IAsyncWork work, TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TResult}(IWorkScheduler,IAsyncWork{TResult},TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TResult>(IAsyncWork<TResult> work, TimeSpan startDelay, TimeSpan repeatDelay,
            int priority = 0, int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork(
                                                                                  work ?? throw new ArgumentNullException(nameof(work)),
                                                                                  startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

#endregion

#region RepeatedDelayedQueueDI

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedQueueWork<TWork>(TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TWork : IWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork<TWork>(startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedQueueWork{TWork,TResult}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedQueueWork<TWork, TResult>(TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TWork : IWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedQueueWork<TWork, TResult>(startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Maybe<Exception>> AddRepeatedAsyncQueueWork<TAsyncWork>(TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork<TAsyncWork>(startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

        /// <inheritdoc cref="WorkSchedulerWorkQueueInteraction.AddRepeatedAsyncQueueWork{TAsyncWork,TResult}(IWorkScheduler,TimeSpan,TimeSpan,int,int,int,CancellationToken)" />
        [PublicAPI]
        public IObservable<Try<TResult>> AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(TimeSpan startDelay, TimeSpan repeatDelay, int priority = 0,
            int attemptsCount = 1, int execCount = -1, CancellationToken cancellation = default)
            where TAsyncWork : IAsyncWork<TResult>
            => (provider ?? throw new ArgumentNullException(nameof(provider))).RequiredService<IWorkScheduler>()
                                                                              .AddRepeatedAsyncQueueWork<TAsyncWork, TResult>(startDelay,
                                                                                  repeatDelay,
                                                                                  priority,
                                                                                  attemptsCount,
                                                                                  execCount,
                                                                                  cancellation);

#endregion
    }
}
