using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core.Interfaces
{
    public interface IAnarchyManagerNew
    {
        /// <summary>
        /// Assigns the specified anarchy action to an ActionOrchestrator
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction to apply the schedule to</param>
        /// <param name="schedule">The schedule to apply</param>
        /// <param name="allowUpdate">Specifies whether or not to allow an update to an existing schedule</param>
        /// <returns>true if the schedule was created and false if updated (only if allowUpdate was true)</returns>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the action orchestrator cannot be found</exception>
        /// <exception cref="UnschedulableActionException">This exception is thrown if the anarchy type does not implement ICauseScheduledAnarchy.</exception>
        /// <exception cref="ScheduleRunningException">This exception is thrown if the action orchestrator containing the anarchy type is currently running a schedule</exception>
        /// <exception cref="ScheduleExistsException">This exception is thrown if the action orchestrator containing the anarchy action has an existing schedule and allowUpdate is false</exception>
        bool AssignScheduleToActionOrchestrator(string anarchyType, Schedule schedule, bool allowUpdate);

        /// <summary>
        /// Assigns the specified anarchy action to an action orchestrator
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction to apply the schedule to</param>
        /// <param name="targetPattern">The target pattern to match against the request url</param>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if a matching action orchestrator cannot be found</exception>
        /// <exception cref="InvalidTargetPatternException">This exception is thrown if the target pattern specified is not a valid .net compatible regex</exception>
        void AssignTargetPattern(string anarchyType, string targetPattern);

        /// <summary>
        /// Retrieves any schedule assigned from the action orchestrator containing the specified action
        /// </summary>
        /// <param name="anarchyType">The type of the AnarchyAction contained within the action orchestrator that contains the schedule.</param>
        /// <returns>An instance of the schedule or null if no schedule has been set.</returns>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the action orchestrator cannot be found</exception>
        Schedule GetScheduleFromActionOrchestrator(string anarchyType);

        /// <summary>
        /// Retrieves a named list of schedules for all schedulable action orchestrators.
        /// </summary>
        /// <returns>A list of NamedScheduleDto for all schedulable action orchestrators.</returns>
        IEnumerable<NamedScheduleDto> GetAllSchedulesFromOrchestrators();

        /// <summary>
        /// Handle any requests via any action orchestrators that are set to handle requests.
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
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the action orchestrator cannot be found</exception>
        /// <exception cref="UnschedulableActionException">This exception is thrown if the anarchy type does not implement ICauseScheduledAnarchy.</exception>
        /// <exception cref="ScheduleRunningException">This exception is thrown if the action orchestrator containing the anarchy type is already running a schedule (and so cannot be triggered directly).</exception>
        void TriggerAction(string anarchyType, TimeSpan? duration);

        /// <summary>
        /// Updates an anarchy action, allowing you to set custom parameters specific to a given action,
        /// this assesses the type based on anarchyType and asks the action to process the provided json payload.
        /// </summary>
        /// <param name="anarchyType">The anarchy type to process</param>
        /// <param name="updatedPayload">Json payload containing the updated properties of the anarchy action
        /// (note it is up to the anarchy action to define how this is processed!</param>
        void UpdateAction(string anarchyType, string updatedPayload);

        /// <summary>
        /// Starts the schedule of an anarchy action orchestrator that contains an anarchy action that is schedulable and has a schedule.
        /// </summary>
        /// <param name="anarchyType">The anarchy type to process</param>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the action orchestrator cannot be found</exception>
        /// <exception cref="ActionStoppingException">this exception is triggered if the action orchestrator has already been asked to stop</exception>
        /// <exception cref="ScheduleRunningException">This exception is thrown if the action orchestrator containing the anarchy type is already running a schedule (and so cannot be triggered directly).</exception>
        /// <exception cref="UnschedulableActionException">This exception is thrown if the anarchy type does not implement ICauseScheduledAnarchy.</exception>
        /// <exception cref="ScheduleMissingException">This exception is thrown if the anarchy action orchestrator does not have a schedule set.</exception>
        void StartSchedule(string anarchyType);

        /// <summary>
        /// Starts the schedules of all the registered anarchy action orchestrators.
        /// </summary>
        /// <exception cref="ActionStoppingException">this exception is triggered if an action orchestrator has already been asked to stop</exception>
        void StartAllSchedules();

        /// <summary>
        /// Stops any scheduled action or currently triggered actions within a given action orchestrator.
        /// NOTE: this can be called against any action orchestrator, as it will kill any in-process action execution (scheduled or not).
        /// </summary>
        /// <param name="anarchyType">The anarchy type to process</param>
        /// <exception cref="AnarchyActionNotFoundException">This exception is thrown if the action orchestrator cannot be found</exception>
        /// <exception cref="ActionStoppingException">this exception is triggered if the action orchestrator has already been asked to stop</exception>
        void StopAction(string anarchyType);

        /// <summary>
        /// Stops all actions indiscriminately - this is the fastest way to stop anarchy.
        /// </summary>
        void StopAllActions();
    }
}