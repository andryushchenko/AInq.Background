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
using System.Threading.Tasks;

namespace AInq.Background.Elements
{

internal static class DelayedWrapperFactory
{
    private class WorkWrapper : ISchedulerWrapper
    {
        private DateTime? _nextScheduledTime;
        private readonly IWork _work;
        private readonly CancellationToken _innerCancellation;

        internal WorkWrapper(IWork work, TimeSpan delay, CancellationToken cancellation = default)
        {
            if (delay.Ticks <= 0)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, null);
            _work = work ?? throw new ArgumentNullException(nameof(work));
            _innerCancellation = cancellation;
            _nextScheduledTime = DateTime.Now.Add(delay);
        }

        internal WorkWrapper(IWork work, DateTime time, CancellationToken cancellation = default)
        {
            if (time <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(time), time, null);
            _work = work ?? throw new ArgumentNullException(nameof(work));
            _innerCancellation = cancellation;
            _nextScheduledTime = time;
        }

        DateTime? ISchedulerWrapper.NextScheduledTime => _nextScheduledTime;

        Task<bool> ISchedulerWrapper.ExecuteAsync(IServiceProvider provider, CancellationToken outerCancellation)
        {
            throw new NotImplementedException();
        }
    }
}

}
