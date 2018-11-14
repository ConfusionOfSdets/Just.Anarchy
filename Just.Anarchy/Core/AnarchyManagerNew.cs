using System.Collections.Generic;
using System.Linq;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;

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

        public Schedule GetScheduleFromAnarchyActionFactory(string anarchyType)
        {
            return GetFactoryContainingAction(anarchyType)?.ExecutionSchedule;
        }

        public IEnumerable<NamedScheduleDto> GetAllSchedulesFromFactories()
        {
            return _actionFactories
                .Where(a => a.AnarchyAction is ICauseScheduledAnarchy)
                .Select(s => new NamedScheduleDto(s.AnarchyAction.Name, s.ExecutionSchedule))
                .OrderBy(n => n.Name);
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