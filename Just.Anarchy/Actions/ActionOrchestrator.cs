using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Extensions;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Actions
{
    public class ActionOrchestrator<TAnarchyAction> : IActionOrchestrator where TAnarchyAction : ICauseAnarchy
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

        public ActionOrchestrator(TAnarchyAction action, IHandleTime timer)
        {
            _timer = timer;
            AnarchyAction = action;
            _executionInstances = new ConcurrentBag<Task>();
            _cancellationTokenSource = new CancellationTokenSource();
            IsActive = false;
        }

        public bool CanHandleRequest(string requestUrl) => _matchTargetPattern != null &&
                                                           _matchTargetPattern.IsMatch(requestUrl);

        public async Task HandleRequest(HttpContext context, RequestDelegate next)
        {
            if (CanHandleRequest(context.Request.Path))
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ActionStoppingException();
                }

                var execution = AnarchyAction.HandleRequestAsync(context, next, _cancellationTokenSource.Token);
                _executionInstances.Add(execution);
                await execution;
            }
        }

        public void TriggerOnce(TimeSpan? duration)
        {
            CheckActionIsSchedulable();

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new ActionStoppingException();
            }

            IsActive = true;

            var task = ((ICauseScheduledAnarchy)AnarchyAction)
                .ExecuteAsync(duration, _cancellationTokenSource.Token)
                .ContinueWith(_ => IsActive = false);

            _executionInstances.Add(task);
        }

        public void Start()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new ActionStoppingException();
            }

            if (ExecutionSchedule != null)
            {
                StartSchedule();
            }

            IsActive = true;
        }

        public async Task Stop()
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
                    throw new EmptyTargetPatternException();
                }

                try
                {
                    _matchTargetPattern = new Regex(pattern, RegexOptions.Compiled & RegexOptions.Singleline, TimeSpan.FromSeconds(1));
                }
                catch (Exception e)
                {
                    throw new InvalidTargetPatternException(pattern, e);
                }
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
