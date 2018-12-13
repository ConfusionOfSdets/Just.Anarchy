using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Extensions;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core
{
    public class AnarchyManagerNew : IAnarchyManagerNew
    {
        private readonly IList<IActionOrchestrator> _actionOrchestrators;

        public AnarchyManagerNew(IEnumerable<IActionOrchestrator> actionOrchestrators)
        {
            this._actionOrchestrators = actionOrchestrators.ToList();
        }

        public bool AssignScheduleToActionOrchestrator(string anarchyType, Schedule schedule, bool allowUpdate)
        {
            var orchestrator = GetOrchestratorContainingAction(anarchyType);

            if (orchestrator.ExecutionSchedule != null && !allowUpdate)
            {
                throw new ScheduleExistsException();
            }

            return orchestrator.AssociateSchedule(schedule);
        }

        public void AssignTargetPattern(string anarchyType, string targetPattern) =>
            GetOrchestratorContainingAction(anarchyType)?.ForTargetPattern(targetPattern);
        

        public Schedule GetScheduleFromActionOrchestrator(string anarchyType) =>
            GetOrchestratorContainingAction(anarchyType)?.ExecutionSchedule;

        public IEnumerable<NamedScheduleDto> GetAllSchedulesFromOrchestrators() =>
            _actionOrchestrators
                .Where(a => a.AnarchyAction.IsOfType<ICauseScheduledAnarchy>())
                .Select(s => new NamedScheduleDto(s.AnarchyAction.Name, s.ExecutionSchedule))
                .OrderBy(n => n.Name);

        public void TriggerAction(string anarchyType, TimeSpan? duration) =>
            GetOrchestratorContainingAction(anarchyType).TriggerOnce(duration);

        public void UpdateAction(string anarchyType, string updatedPayload)
        {
            var orchestrator = GetOrchestratorContainingAction(anarchyType);
            orchestrator.UpdateAction(updatedPayload);
        }

        public async Task<bool> HandleRequest(HttpContext context, RequestDelegate next)
        {
            var passiveRequestOrchestrators = _actionOrchestrators.Where(a => a.AnarchyAction.AnarchyType == CauseAnarchyType.Passive &&
                                                                         a.CanHandleRequest(context.Request.Path))
                                                                  .ToList();

            var alterResponseOrchestrators = _actionOrchestrators.Where(a => a.AnarchyAction.AnarchyType == CauseAnarchyType.AlterResponse &&
                                                                        a.CanHandleRequest(context.Request.Path))
                                                                 .ToList();

            // if we have >1 orchestrator that alters the response, throw an exception and don't run anything, there's an error with config.
            if (alterResponseOrchestrators.Count() > 1)
            {
                throw new MultipleResponseAlteringActionsEnabledException(alterResponseOrchestrators.Select(a => a.AnarchyAction.Name).ToList());
            }

            // Run all passive request orchestrators in parallel without awaiting
            passiveRequestOrchestrators.Select(p => p.HandleRequest(context, next)).ToList();

            //If there are no response-altering actions, return false
            if (!alterResponseOrchestrators.Any()) return false;

            //Run the response altering requests and return true to say the response has been altered
            await alterResponseOrchestrators[0].HandleRequest(context, next);
            return true;
        }

        public void StartSchedule(string anarchyType) => GetOrchestratorContainingAction(anarchyType).Start();

        public void StartAllSchedules()
        {
            Parallel.ForEach(
                _actionOrchestrators.Where(a => a.AnarchyAction is ICauseScheduledAnarchy),
                StartIgnoringScheduleErrors
            );
        }

        public void StopAction(string anarchyType) => GetOrchestratorContainingAction(anarchyType).Stop();

        public void StopAllActions() => Parallel.ForEach(_actionOrchestrators, o => o.Stop());

        private void StartIgnoringScheduleErrors(IActionOrchestrator orchestrator)
        {
            try
            {
                orchestrator.Start();
            }
            catch (ScheduleMissingException)
            {
                // Deliberately suppress this
                //TODO: Should we provide feedback in this instance or error here?
            }
        }

        private IActionOrchestrator GetOrchestratorContainingAction(string anarchyType)
        {
            var actionOrchestrator = _actionOrchestrators.FirstOrDefault(o => o.AnarchyAction.Name.ToLower().Equals(anarchyType?.ToLower()));

            if (actionOrchestrator == null)
            {
                throw new AnarchyActionNotFoundException(anarchyType);
            }

            return actionOrchestrator;
        }
    }
}