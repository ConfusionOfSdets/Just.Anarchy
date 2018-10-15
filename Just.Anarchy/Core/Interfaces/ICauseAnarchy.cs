using System;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy
{
    public interface ICauseAnarchy
    {
        CauseAnarchyType AnarchyType { get; set; }
        string Name { get; }
        bool Active { get; set; }
        int StatusCode { get; }
        string Body { get; }

        /// <summary>
        /// Execute one execution of the specified AnarchyAction, if specified the action should limit it's execution duration to the specified duration.
        /// </summary>
        /// <param name="duration">The amount of time the AnarchyAction will run, if null the AnarchyAction will continue to run until disabled.</param>
        /// <param name="cancellationToken">If triggered, the action should respect cancellation and stop.</param>
        /// <returns>Task performing the AnarchyAction</returns>
        Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken);
    }
}