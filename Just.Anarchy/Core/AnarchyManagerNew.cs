using System.Collections.Generic;
using System.Linq;
using Just.Anarchy.Exceptions;

namespace Just.Anarchy.Core
{
    public class AnarchyManagerNew
        : IAnarchyManagerNew
    {
        private readonly IList<IAnarchyActionFactory> _actionFactories;

        public AnarchyManagerNew(IEnumerable<IAnarchyActionFactory> actionFactories)
        {
            this._actionFactories = actionFactories.ToList();
        }

        public void AssignScheduleToAnarchyActionFactory(string anarchyType, Schedule schedule)
        {
            var factory = GetFactoryContainingAction(anarchyType);

            if (factory.ExecutionSchedule != null)
            {
                throw new ScheduleExistsException();
            }

            factory.AssociateSchedule(schedule);
        }

        public Schedule GetScheduleFromAnarchyActionFactory(string anarchyType)
        {
            return GetFactoryContainingAction(anarchyType)?.ExecutionSchedule;
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