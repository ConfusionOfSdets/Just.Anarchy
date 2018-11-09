using System.Collections.Generic;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Exceptions;

namespace Just.Anarchy.Core.Interfaces
{
    public interface IAnarchyManagerNew
    {
        /// <summary>
        /// Assigns the specified anarchy action to an anarchyActionFactory
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction to apply the schedule to</param>
        /// <param name="schedule">The schedule to apply</param>
        /// <param name="allowUpdate">Specifies whether or not to allow an update to an existing schedule</param>
        /// <returns>true if the schedule was created and false if updated (only if allowUpdate was true)</returns>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the anarchy action factory cannot be found</exception>
        /// <exception cref="UnschedulableActionException">This exception is thrown if the anarchy type does not implement ICauseScheduledAnarchy.</exception>
        /// <exception cref="ScheduleRunningException">This exception is thrown if the anarchy action factory containing the anarchy type is currently running a schedule</exception>
        /// <exception cref="ScheduleExistsException">This exception is thrown if the anarchy action factory containing the anarchy action has an existing schedule and allowUpdate is false</exception>
        bool AssignScheduleToAnarchyActionFactory(string anarchyType, Schedule schedule, bool allowUpdate);

        /// <summary>
        /// Retrieves any schedule assigned from the  anarchy action factory containing the specified action
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction contained within the factory that contains the schedule.</param>
        /// <returns>An instance of the schedule or null if no schedule has been set.</returns>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the anarchy action factory cannot be found</exception>
        Schedule GetScheduleFromAnarchyActionFactory(string anarchyType);

        /// <summary>
        /// Retrieves a named list of schedules for all schedulable anarchy action factories
        /// </summary>
        /// <returns>A list of NamedScheduleDto for all schedulable anarchyaction factories.</returns>
        IEnumerable<NamedScheduleDto> GetAllSchedulesFromFactories();
    }
}