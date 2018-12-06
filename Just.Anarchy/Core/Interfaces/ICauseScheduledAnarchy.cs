using System;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy.Core.Interfaces
{
    public interface ICauseScheduledAnarchy : ICauseAnarchy
    {
        TimeSpan DefaultDuration { get; }

        /// <summary>
        /// Execute one execution of the specified AnarchyAction, if specified the action should limit it's execution duration to the specified duration.
        /// </summary>
        /// <param name="duration">The amount of time the AnarchyAction will run, if null the AnarchyAction will continue to run until disabled.</param>
        /// <param name="cancellationToken">If triggered, the action should respect cancellation and stop.</param>
        /// <returns>Task performing the AnarchyAction</returns>
        Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken);
    }
}