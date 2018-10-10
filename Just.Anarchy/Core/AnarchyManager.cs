using System;
using System.Collections.Generic;
using System.Linq;

namespace Just.Anarchy
{
    public class AnarchyManager : IAnarchyManager
    {
        private readonly IList<IAnarchyActionFactory> _actionFactories;

        public AnarchyManager(IEnumerable<IAnarchyActionFactory> actionFactories)
        {
            this._actionFactories = actionFactories.ToList();
        }
        private AnarchyState State { get; set; }

        public  AnarchyState GetState()
        {
            return State;
        }

        public void EnableAnarchy()
        {
            State = AnarchyState.Active;
            UpdateAllFactoryStates(true);
        }

        public void DisableAnarchy()
        {
            State = AnarchyState.Disabled;
            UpdateAllFactoryStates(false);
        }

        private void UpdateAllFactoryStates(bool active)
        {
            foreach (var actionFactory in _actionFactories)
            {
                if (active)
                {
                    actionFactory.Start();
                }
                else
                {
                    actionFactory.Stop();
                }
            }
        }

        public IAnarchyActionFactory ChooseRandomAnarchyActionFactory()
        {
            var random = new Random();
            var activeActions = _actionFactories.Where(x => x.IsActive).ToList();
            return activeActions[random.Next(0, activeActions.Count)];
        }

        public void EnableSpecificType(string anarchyType)
        {
            var actionFactory = GetActionFactoryCalled(anarchyType);
            if (actionFactory == null) return;
            actionFactory.Start();
            State = AnarchyState.Active;
        }

        public void SetRequestPatternForType(string anarchyType, string requestPattern)
        {
            var actionFactory = GetActionFactoryCalled(anarchyType);
            actionFactory?.ForTargetPattern(requestPattern);
        }

        public List<IAnarchyActionFactory> GetAllActiveActionFactories()
        {
            return _actionFactories.Where(x => x.IsActive).ToList();
        }

        public List<IAnarchyActionFactory> GetAllInactiveActionFactories()
        {
            return _actionFactories.Where(x => !x.IsActive).ToList();
        }

        private IAnarchyActionFactory GetActionFactoryCalled(string anarchyType) =>
            _actionFactories.FirstOrDefault(x => x.AnarchyAction.GetType().Name.Contains(anarchyType));
    }
}