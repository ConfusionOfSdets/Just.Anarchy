using System;

namespace Just.Anarchy.Exceptions
{
    public class ScheduleRunningException : Exception
    {
        public ScheduleRunningException() : base("The ActionOrchestrator is active and running a schedule, stop the ActionOrchestrator before setting a new schedule")
        {
        }
    }
}
