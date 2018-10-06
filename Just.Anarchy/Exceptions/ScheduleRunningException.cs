using System;

namespace Just.Anarchy.Exceptions
{
    public class ScheduleRunningException : Exception
    {
        public ScheduleRunningException() : base("The AnarchyActionFactory is active and running a schedule, stop the factory before setting a new schedule")
        {
        }
    }
}
