using System;
using System.Collections.Generic;
using System.Linq;

namespace Just.Anarchy
{
    public class AnarchyManager : IAnarchyManager
    {
        private readonly IList<ICauseAnarchy> _anarchyActions;

        public AnarchyManager(IEnumerable<ICauseAnarchy> chaosActions)
        {
            this._anarchyActions = chaosActions.ToList();
        }
        private AnarchyState State { get; set; }

        public  AnarchyState GetState()
        {
            return State;
        }

        public void EnableAnarchy()
        {
            State = AnarchyState.Active;
            UpdateAllActions(true);
        }

        public void DisableAnarchy()
        {
            State = AnarchyState.Disabled;
            UpdateAllActions(false);
        }

        private void UpdateAllActions(bool active)
        {
            foreach (var anarchyAction in _anarchyActions)
            {
                anarchyAction.Active = active;
            }
        }

        public ICauseAnarchy ChooseRandomAnarchyAction()
        {
            var random = new Random();
            var activeActions = _anarchyActions.Where(x => x.Active).ToList();
            return activeActions[random.Next(0, activeActions.Count)];
        }

        public void EnableSpecificType(string anarchyType)
        {
            var action = _anarchyActions.FirstOrDefault(x => x.GetType().Name.Contains(anarchyType));
            if (action == null) return;
            action.Active = true;
            State = AnarchyState.Active;
        }

        public List<ICauseAnarchy> GetAllActiveActions()
        {
            return _anarchyActions.Where(x => x.Active).ToList();
        }

        public List<ICauseAnarchy> GetAllInactiveActions()
        {
            return _anarchyActions.Where(x => !x.Active).ToList();
        }
    }
}