using System;

namespace Just.Anarchy.Exceptions
{
    public class ScheduleExistsException : Exception
    {
        public ScheduleExistsException() : base("The AnarchyActionFactory already has a schedule, try to update the schedule instead.")
        {
        }
    }
}
