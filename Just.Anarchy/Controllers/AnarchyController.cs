using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Just.Anarchy.Controllers
{
    public class AnarchyController : Controller
    {
        private readonly IAnarchyManagerNew _anarchyManager;

        public AnarchyController(IAnarchyManagerNew anarchyManager)
        {
            _anarchyManager = anarchyManager;
        }

        [HttpPost, Route(Routes.Anarchy.SetOrCancelOnRequestHandling)]
        public IActionResult SetActionTargetPattern(string anarchyType, [FromBody]EnableOnRequestHandlingRequest request)
        {
            if (request == null)
            {
                throw new RequestBodyRequiredException<EnableOnRequestHandlingRequest>();
            }

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

        [HttpPost, Route(Routes.Anarchy.UpdateAction)]
        public async Task<IActionResult> UpdateAction(string anarchyType)
        {
            string updatedAnarchyJson = null;

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                updatedAnarchyJson = await reader.ReadToEndAsync();
            }

            _anarchyManager.UpdateAction(anarchyType, updatedAnarchyJson);
            return new OkResult();
        }

        //[Route("status/anarchy/state")]
        //public object GetState()
        //{
        //    return new
        //    {
        //        State=_anarchyManager.GetState().ToString(), 
        //        ActiveActions=_anarchyManager.GetAllActiveActionOrchestrators().Select(f => f.AnarchyAction),
        //        InActiveActions=_anarchyManager.GetAllInactiveActionOrchestrators().Select(f => f.AnarchyAction)
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