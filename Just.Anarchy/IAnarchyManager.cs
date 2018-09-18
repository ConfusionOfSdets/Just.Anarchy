using System.Collections.Generic;

namespace Just.Anarchy
{
    public interface IAnarchyManager
    {
        AnarchyState GetState();
        void EnableAnarchy();
        void DisableAnarchy();
        ICauseAnarchy ChooseRandomAnarchyAction();
        void EnableSpecificType(string anarchytype);
        List<ICauseAnarchy> GetAllActiveActions();
        List<ICauseAnarchy> GetAllInactiveActions();
    }
}