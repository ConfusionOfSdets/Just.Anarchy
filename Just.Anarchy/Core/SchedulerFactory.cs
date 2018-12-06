using Just.Anarchy.Core.Interfaces;

namespace Just.Anarchy.Core
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private readonly IHandleTime _timer;

        public SchedulerFactory(IHandleTime timer)
        {
            _timer = timer;
        }

        public IScheduler CreateSchedulerForAction(Schedule schedule, ICauseScheduledAnarchy action) => new Scheduler(schedule, action, _timer);
    }
}
