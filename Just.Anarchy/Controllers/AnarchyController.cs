using System;
using System.Linq;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Just.Anarchy.Controllers
{
    public class AnarchyController : Controller
    {
        private readonly IAnarchyManagerNew _anarchyManager;

        public AnarchyController(IAnarchyManagerNew anarchyManager)
        {
            _anarchyManager = anarchyManager;
        }

        [HttpPost, Route(Routes.Anarchy.SetOnRequestHandling)]
        public IActionResult SetActionTargetPattern(string anarchyType, [FromBody]EnableOnRequestHandlingRequest request)
        { 
            //TODO: Validate request?
            _anarchyManager.AssignTargetPattern(anarchyType, request.TargetPattern);

            return new OkResult();
        }

        [HttpPost, Route(Routes.Anarchy.Trigger)]
        public IActionResult TriggerAction(string anarchyType, int? durationSecs)
        {
            var duration = durationSecs.HasValue ?
                TimeSpan.FromSeconds(durationSecs.Value) :
                (TimeSpan?)null;

            _anarchyManager.TriggerAction(anarchyType, duration);
            return new AcceptedResult();
        }

        //[Route("status/anarchy/state")]
        //public object GetState()
        //{
        //    return new
        //    {
        //        State=_anarchyManager.GetState().ToString(), 
        //        ActiveActions=_anarchyManager.GetAllActiveActionFactories().Select(f => f.AnarchyAction),
        //        InActiveActions=_anarchyManager.GetAllInactiveActionFactories().Select(f => f.AnarchyAction)
        //    };
        //}

        //[Route("status/anarchy/enable")]
        //public void EnableAnarchy()
        //{
        //    _anarchyManager.EnableAnarchy();
        //}

        //[Route("status/anarchy/disable")]
        //public void DisableAnarchy()
        //{
        //    _anarchyManager.DisableAnarchy();
        //}

        //[Route("status/anarchy/enable/{anarchytype}")]
        //public void EnableIndividualAnarchy([FromRoute] string anarchytype, string requestPattern = null)
        //{
        //    _anarchyManager.EnableSpecificType(anarchytype);
        //    if (requestPattern != null)
        //    {
        //        _anarchyManager.SetRequestPatternForType(anarchytype, requestPattern);
        //    }
        //}
    }
}