using System;
using System.Collections.Generic;
using Just.Anarchy.Exceptions;

namespace Just.Anarchy
{
    public interface IAnarchyManagerNew
    {
        /// <summary>
        /// Assigns the specified anarchy action to an anarchyActionFactory
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction to apply the schedule to</param>
        /// <param name="schedule">The schedule to apply</param>
        /// <exception cref="UnschedulableActionException">This exception is thrown if the anarchy type does not implement ICauseScheduledAnarchy.</exception>
        /// <exception cref="ScheduleRunningException">This exception is thrown if the anarchy action factory containing the anarchy type is currently running a schedule</exception>
        void AssignScheduleToAnarchyActionFactory(string anarchyType, Schedule schedule);
    }
}