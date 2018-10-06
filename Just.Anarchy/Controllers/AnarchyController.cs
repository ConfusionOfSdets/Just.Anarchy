using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Just.Anarchy.Controllers
{
    public class AnarchyController : Controller
    {
        private readonly IAnarchyManager _anarchyManager;

        public AnarchyController(IAnarchyManager anarchyManager)
        {
            _anarchyManager = anarchyManager;
        }

        [Route("status/anarchy/state")]
        public object GetState()
        {
            return new
            {
                State=_anarchyManager.GetState().ToString(), 
                ActiveActions=_anarchyManager.GetAllActiveActionFactories().Select(f => f.AnarchyAction),
                InActiveActions=_anarchyManager.GetAllInactiveActionFactories().Select(f => f.AnarchyAction)
            };
        }

        [Route("status/anarchy/enable")]
        public void EnableAnarchy()
        {
            _anarchyManager.EnableAnarchy();
        }

        [Route("status/anarchy/disable")]
        public void DisableAnarchy()
        {
            _anarchyManager.DisableAnarchy();
        }

        [Route("status/anarchy/enable/{anarchytype}")]
        public void EnableIndividualAnarchy([FromRoute] string anarchytype, string requestPattern = null)
        {
            _anarchyManager.EnableSpecificType(anarchytype);
            if (requestPattern != null)
            {
                _anarchyManager.SetRequestPatternForType(anarchytype, requestPattern);
            }
        }
    }
}