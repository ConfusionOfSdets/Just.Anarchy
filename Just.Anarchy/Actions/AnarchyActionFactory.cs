using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Extensions;

namespace Just.Anarchy.Actions
{
    public class AnarchyActionFactory : IAnarchyActionFactory
    {
        
        public ICauseAnarchy AnarchyAction { get; }
        public bool IsActive { get; private set; }
        public string TargetPattern => _matchTargetPattern.ToString();
        public Schedule ExecutionSchedule { get; private set; }

        private ConcurrentBag<Task> _executionInstances;
        private CancellationTokenSource _cancellationTokenSource;
        
        private Regex _matchTargetPattern;
        private IScheduler _scheduler;
        private readonly IHandleTime _timer;

        public AnarchyActionFactory(ICauseAnarchy anarchyAction, IHandleTime timer)
        {
            _timer = timer;
            AnarchyAction = anarchyAction;
            _executionInstances = new ConcurrentBag<Task>();
            _cancellationTokenSource = new CancellationTokenSource();
            IsActive = false;
        }

        public void HandleRequest(string requestUrl)
        {
            if (ShouldHandleRequest(requestUrl))
            {
                if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                var execution = AnarchyAction.ExecuteAsync(ExecutionSchedule?.IterationDuration, _cancellationTokenSource.Token);
                _executionInstances.Add(execution);
            }
        }

        public void TriggerOnce(TimeSpan? duration)
        {
            CheckActionIsSchedulable();

            IsActive = true;

            var task = AnarchyAction
                .ExecuteAsync(duration, _cancellationTokenSource.Token)
                .ContinueWith(_ => IsActive = false);

            _executionInstances.Add(task);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            if (ExecutionSchedule != null)
            {
                StartSchedule();
            }

            IsActive = true;
        }

        public async void Stop()
        {
            _cancellationTokenSource?.Cancel();

            _scheduler?.StopSchedule();
            await StopUnscheduledExecutions();

            _cancellationTokenSource = new CancellationTokenSource();
            IsActive = false;
        }

        public bool AssociateSchedule(Schedule schedule)
        {
            CheckActionIsSchedulable();
            CheckScheduleIsNotRunning();

            var created = ExecutionSchedule == null;

            ExecutionSchedule = schedule;

            return created;
        }

        public void ForTargetPattern(string pattern)
        {
            if (pattern == null)
            {
                _matchTargetPattern = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pattern))
                {
                    throw new ArgumentException("The target pattern needs to be a valid .net regular expression");
                }
                _matchTargetPattern = new Regex(pattern, RegexOptions.Compiled & RegexOptions.Singleline, TimeSpan.FromSeconds(1));
            }    
        }

        private void StartSchedule()
        {
            if (AnarchyAction is ICauseScheduledAnarchy scheduledAction)
            {
                _scheduler = new Scheduler(ExecutionSchedule, scheduledAction, _timer);
                _scheduler.StartSchedule();
            }
        }

        private void CheckActionIsSchedulable()
        {
            if (AnarchyAction.IsNotOfType<ICauseScheduledAnarchy>())
            {
                throw new UnschedulableActionException();
            }
        }

        private void CheckScheduleIsNotRunning()
        {
            if (IsActive)
            {
                throw new ScheduleRunningException();
            }
        }

        private bool ShouldHandleRequest(string requestUrl) => 
            _matchTargetPattern != null && 
            _matchTargetPattern.IsMatch(requestUrl);

        private async Task StopUnscheduledExecutions()
        {
            while (_executionInstances.Any(x => !x.IsCompleted))
            {
                await Task.Delay(100);
            }

            _executionInstances = new ConcurrentBag<Task>();
        }

    }
}
