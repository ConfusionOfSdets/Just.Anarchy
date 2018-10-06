using System;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy
{
    public class Timer : IHandleTime
    {
        public Task DelayInitial(Schedule schedule, CancellationToken ct) => Task.Delay(schedule.Delay, ct);

        public Task DelayInterval(Schedule schedule, CancellationToken ct) => Task.Delay(schedule.Interval, ct);
    }
}
