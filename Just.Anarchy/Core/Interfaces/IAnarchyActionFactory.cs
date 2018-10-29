using System;
using System.Threading.Tasks;
using Just.Anarchy.Exceptions;

namespace Just.Anarchy
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
        /// Used by the AnarchyMiddleware to trigger per-request action execution.
        /// It is up to the AnarchyActionFactory to decide whether or not to trigger the action based on the url.
        /// </summary>
        /// <param name="requestUrl">The URL to evaluate against the TargetPattern</param>
        void HandleRequest(string requestUrl);

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
        /// <exception cref="ScheduleRunningException">Thrown if a schedule is running when this is called</exception>
        /// <exception cref="UnschedulableActionException">Thrown if the AnarchyActionFactory does not contain an action that implements ICauseScheduledAnarchy</exception>
        void AssociateSchedule(Schedule schedule);

        /// <summary>
        /// Set a target pattern to match, this is null by default and will be ignored on request.
        /// '*' means apply to ALL requests
        /// '/bob' means apply to all requests that come on the path '/bob'
        /// </summary>
        /// <param name="pattern">The url path pattern to match</param>
        void ForTargetPattern(string pattern);
    }
}