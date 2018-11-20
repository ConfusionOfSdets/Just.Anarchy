using System;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core.Interfaces
{
    /// <summary>
    /// Provides an interface to specify how an action factory behaves
    /// </summary>
    public interface IAnarchyActionFactory
    {
        /// <summary>
        /// The action to trigger
        /// </summary>
        ICauseAnarchy AnarchyAction { get; }

        bool IsActive { get; }

        /// <summary>
        /// Returns the current TargetPattern set
        /// </summary>
        string TargetPattern { get; }

        /// <summary>
        /// Returns the current execution schedule (note: if TargetPattern is set the behaviour of this alters)
        /// </summary>
        Schedule ExecutionSchedule { get; }

        /// <summary>
        /// Used by the AnarchyMiddleware to check if an AnarchyActionFactory can handle a request at a given path based on
        /// the TargetPattern and enabled state.
        /// </summary>
        /// <param name="requestUrl">The URL to evaluate against the TargetPattern</param>
        /// <returns>true if the AnarchyActionFactory can handle the request and false if it cannot.</returns>
        bool CanHandleRequest(string requestUrl);

        /// <summary>
        /// Used by the AnarchyMiddleware to trigger per-request action execution.
        /// It is up to the AnarchyActionFactory to decide whether or not to trigger the action based on the url.
        /// </summary>
        /// <param name="requestUrl">The URL to evaluate against the TargetPattern</param>
        /// <param name="next">The request delegate to call after performing the action</param>
        Task HandleRequest(HttpContext context, RequestDelegate next);

        /// <summary>
        /// Start the factory, this will validate the schedule and error if invalid.
        /// If ForTargetPattern is set, it will trigger on requests
        /// otherwise it will trigger based on the schedule set.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the factory, all current actions will be asked to terminate,
        /// any request actions will continue until complete.
        /// </summary>
        void Stop();

        /// <summary>
        /// Associates a schedule with the action factory - some aspects of this will be ignored if TargetPattern is set.
        /// The schedule will be validated at this point, invalid schedules will be rejected, if an existing schedule is set it will be overwritten.
        /// </summary>
        /// <param name="schedule">The schedule to add</param>
        /// <returns>true if the schedule was created and false if the schedule was updated.</returns>
        /// <exception cref="ScheduleRunningException">Thrown if a schedule is running when this is called</exception>
        /// <exception cref="UnschedulableActionException">Thrown if the AnarchyActionFactory does not contain an action that implements ICauseScheduledAnarchy</exception>
        bool AssociateSchedule(Schedule schedule);

        /// <summary>
        /// Performs a one-off execution of the action, only applicable to actions of type ICauseScheduledAnarchy, also will not fire if there is an active schedule.
        /// </summary>
        /// <param name="duration">The length of time to execute the action</param>
        /// <exception cref="ScheduleRunningException">Thrown if a schedule is running when this is called</exception>
        /// <exception cref="UnschedulableActionException">Thrown if the AnarchyActionFactory does not contain an action that implements ICauseScheduledAnarchy</exception>
        void TriggerOnce(TimeSpan? duration);

        /// <summary>
        /// Set a target pattern to match, this is null by default and will be ignored on request.
        /// '/bob' means apply to all requests that come on the path '/bob'
        /// This must be a valid .net compatible regex or null, whitespace ar an empty string will be rejected.
        /// </summary>
        /// <param name="pattern">The url path regex pattern to match (.net compatible regex or null)</param>
        /// <exception cref="InvalidTargetPatternException">Thrown if the targetPattern is whitespace or an invalid regex.</exception>
        void ForTargetPattern(string pattern);
    }
}