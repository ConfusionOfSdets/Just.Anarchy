namespace Just.Anarchy.Core.Interfaces
{
    public interface ISchedulerFactory
    {
        IScheduler CreateSchedulerForAction(Schedule schedule, ICauseScheduledAnarchy action);
    }
}