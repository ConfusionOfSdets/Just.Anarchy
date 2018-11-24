using System;
using System.Collections.Generic;
using System.Linq;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;

namespace Just.Anarchy.Core
{
    public class AnarchyManager : IAnarchyManager
    {
        private readonly IList<IActionOrchestrator> _actionOrchestrators;

        public AnarchyManager(IEnumerable<IActionOrchestrator> actionOrchestrators)
        {
            this._actionOrchestrators = actionOrchestrators.ToList();
        }
        private AnarchyState State { get; set; }

        public  AnarchyState GetState()
        {
            return State;
        }

        public void EnableAnarchy()
        {
            State = AnarchyState.Active;
            UpdateAllActionOrchestratorStates(true);
        }

        public void DisableAnarchy()
        {
            State = AnarchyState.Disabled;
            UpdateAllActionOrchestratorStates(false);
        }

        private void UpdateAllActionOrchestratorStates(bool active)
        {
            foreach (var actionOrchestrator in _actionOrchestrators)
            {
                if (active)
                {
                    actionOrchestrator.Start();
                }
                else
                {
                    actionOrchestrator.Stop();
                }
            }
        }

        public IActionOrchestrator ChooseRandomActionOrchestrator()
        {
            var random = new Random();
            var activeActions = _actionOrchestrators.Where(x => x.IsActive).ToList();
            return activeActions[random.Next(0, activeActions.Count)];
        }

        public void EnableSpecificType(string anarchyType)
        {
            var actionOrchestrator = GetActionOrchestratorCalled(anarchyType);
            if (actionOrchestrator == null) return;
            actionOrchestrator.Start();
            State = AnarchyState.Active;
        }

        public void SetRequestPatternForType(string anarchyType, string requestPattern)
        {
            var actionOrchestrator = GetActionOrchestratorCalled(anarchyType);
            actionOrchestrator?.ForTargetPattern(requestPattern);
        }

        public List<IActionOrchestrator> GetAllActiveActionOrchestrators()
        {
            return _actionOrchestrators.Where(x => x.IsActive).ToList();
        }

        public List<IActionOrchestrator> GetAllInactiveActionOrchestrators()
        {
            return _actionOrchestrators.Where(x => !x.IsActive).ToList();
        }

        private IActionOrchestrator GetActionOrchestratorCalled(string anarchyType) =>
            _actionOrchestrators.FirstOrDefault(x => x.AnarchyAction.GetType().Name.Contains(anarchyType));
    }
}