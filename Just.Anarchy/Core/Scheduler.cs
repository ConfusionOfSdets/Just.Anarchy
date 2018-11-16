using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;

namespace Just.Anarchy.Core
{
    public class Scheduler : IScheduler
    {
        public Schedule Schedule { get; private set; }
        public bool Running => !(_scheduleTask?.IsCompleted ?? false);

        private readonly ICauseScheduledAnarchy _action;
        private Task _scheduleTask;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private readonly IHandleTime _timer;

        public Scheduler(Schedule schedule, ICauseScheduledAnarchy action, IHandleTime timer)
        {
            this.Schedule = schedule;
            this._action = action;
            this._timer = timer;
        }

        public void StartSchedule()
        {
            _cancellationTokenSource = (Schedule.TotalDuration != null)
                ? new CancellationTokenSource((int) Schedule.TotalDuration.Value.TotalMilliseconds)
                : new CancellationTokenSource();

            _cancellationToken = _cancellationTokenSource.Token;

            _scheduleTask = Task.Run(async () =>
            {
                // Add initial delay
                await _timer.DelayInitial(Schedule, _cancellationToken);

                var remaining = Schedule.RepeatCount;
                var firstRun = true;
                while (
                    !_cancellationToken.IsCancellationRequested &&
                    (firstRun || Schedule.RepeatsForever|| remaining > 0)
                )
                {
                    await StartScheduledInstance();
                    remaining--;

                    if (remaining > 0 || Schedule.RepeatsForever)
                    {
                        await _timer.DelayInterval(Schedule, _cancellationToken);
                    }

                    firstRun = false;
                    
                }

            }, _cancellationToken);
        }

        public void StopSchedule()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task StartScheduledInstance()
        {
            var iterationDuration = Schedule?.IterationDuration;
            await _action.ExecuteAsync(iterationDuration, _cancellationToken);
        }
    }
}
