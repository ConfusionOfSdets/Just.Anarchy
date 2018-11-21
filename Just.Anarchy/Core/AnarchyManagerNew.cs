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
        private readonly IList<IAnarchyActionFactory> _actionFactories;

        public AnarchyManagerNew(IEnumerable<IAnarchyActionFactory> actionFactories)
        {
            this._actionFactories = actionFactories.ToList();
        }

        public bool AssignScheduleToAnarchyActionFactory(string anarchyType, Schedule schedule, bool allowUpdate)
        {
            var factory = GetFactoryContainingAction(anarchyType);

            if (factory.ExecutionSchedule != null && !allowUpdate)
            {
                throw new ScheduleExistsException();
            }

            return factory.AssociateSchedule(schedule);
        }

        public void AssignTargetPattern(string anarchyType, string targetPattern)
        {
            var factory = GetFactoryContainingAction(anarchyType);
            factory.ForTargetPattern(targetPattern);
        }

        public Schedule GetScheduleFromAnarchyActionFactory(string anarchyType)
        {
            return GetFactoryContainingAction(anarchyType)?.ExecutionSchedule;
        }

        public IEnumerable<NamedScheduleDto> GetAllSchedulesFromFactories()
        {
            return _actionFactories
                .Where(a => a.AnarchyAction.IsOfType<ICauseScheduledAnarchy>())
                .Select(s => new NamedScheduleDto(s.AnarchyAction.Name, s.ExecutionSchedule))
                .OrderBy(n => n.Name);
        }

        public void TriggerAction(string anarchyType, TimeSpan? duration)
        {
            var factory = GetFactoryContainingAction(anarchyType);
            factory.TriggerOnce(duration);
        }

        public async Task<bool> HandleRequest(HttpContext context, RequestDelegate next)
        {
            var passiveRequestFactories = _actionFactories.Where(a => a.AnarchyAction.AnarchyType == CauseAnarchyType.Passive &&
                                                                      a.CanHandleRequest(context.Request.Path));

            var alterResponseFactories = _actionFactories.Where(a => a.AnarchyAction.AnarchyType == CauseAnarchyType.AlterResponse &&
                                                                     a.CanHandleRequest(context.Request.Path))
                                                         .ToList();

            // if we have >1 factory that alters the response, throw an exception and don't run anything, there's an error with config.
            if (alterResponseFactories.Count() > 1)
            {
                throw new MultipleResponseAlteringActionsEnabledException(alterResponseFactories.Select(a => a.AnarchyAction.Name).ToList());
            }

            // Run all passive request factories in parallel without awaiting
            passiveRequestFactories.Select(p => p.HandleRequest(context, next)).ToList();

            //Run the response altering requests (if any)
            if (alterResponseFactories.Any())
            {
                await alterResponseFactories[0].HandleRequest(context, next);
                return true;
            }

            return false;
        }

        private IAnarchyActionFactory GetFactoryContainingAction(string anarchyType)
        {
            var factory = _actionFactories.FirstOrDefault(f => f.AnarchyAction.Name.ToLower().Equals(anarchyType?.ToLower()));

            if (factory == null)
            {
                throw new AnarchyActionNotFoundException(anarchyType);
            }

            return factory;
        }
    }
}