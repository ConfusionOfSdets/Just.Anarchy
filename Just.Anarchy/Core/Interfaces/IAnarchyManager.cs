using System.Collections.Generic;
using Just.Anarchy.Core.Enums;

namespace Just.Anarchy.Core.Interfaces
{
    public interface IAnarchyManager
    {
        AnarchyState GetState();
        void EnableAnarchy();
        void DisableAnarchy();
        IActionOrchestrator ChooseRandomActionOrchestrator();
        void EnableSpecificType(string anarchytype);
        void SetRequestPatternForType(string anarchytype, string requestPattern);
        List<IActionOrchestrator> GetAllActiveActionOrchestrators();
        List<IActionOrchestrator> GetAllInactiveActionOrchestrators();
    }
}