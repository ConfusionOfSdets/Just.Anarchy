﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Exceptions;
using Microsoft.AspNetCore.Http;

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
        /// Assigns the specified anarchy action to an anarchyActionFactory
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction to apply the schedule to</param>
        /// <param name="targetPattern">The target pattern to match against the request url</param>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the anarchy action factory cannot be found</exception>
        void AssignTargetPattern(string anarchyType, string targetPattern);

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
        /// <returns>A list of NamedScheduleDto for all schedulable AnarchyAction factories.</returns>
        IEnumerable<NamedScheduleDto> GetAllSchedulesFromFactories();

        /// <summary>
        /// Handle any requests via any action factories that are set to handle requests.
        /// </summary>
        /// <para>If the handling action is of CauseAnarchyType.Passive it will then run the next RequestDelegate.</para>
        /// <para>If the handling action is of type CauseAnarchyType.AlterResponse it will write a response and finish.</para>
        /// <param name="context">The current httpContext</param>
        /// <param name="next">The next requestDelegate</param>
        /// <returns>True if response has not been manipulated and false if the response has been written.</returns>
        Task<bool> HandleRequest(HttpContext context, RequestDelegate next);

        /// <summary>
        /// Triggers a schedulable anarchy action immediately.
        /// </summary>
        /// <param name="anarchyType">The anarchy type to trigger</param>
        /// <param name="duration">The time to run the anarchy action for, if null this will be set to a default defined in the underlying anarchy action.</param>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the anarchy action factory cannot be found</exception>
        /// <exception cref="UnschedulableActionException">This exception is thrown if the anarchy type does not implement ICauseScheduledAnarchy.</exception>
        /// <exception cref="ScheduleRunningException">This exception is thrown if the anarchy action factory containing the anarchy type is already running a schedule (and so cannot be triggered directly).</exception>
        void TriggerAction(string anarchyType, TimeSpan? duration);
    }
}