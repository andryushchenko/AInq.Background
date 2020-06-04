using System;
using System.Threading;
using System.Threading.Tasks;

namespace AInq.Support.Background.Elements
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