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

        [HttpPut, Route(Routes.Anarchy.StopAction)]
        public IActionResult StopAction(string anarchyType)
        {
            _anarchyManager.StopAction(anarchyType);

            //TODO: When working return url to Get State of AnarchyAction here...
            return new AcceptedResult();
        }

        [HttpPut, Route(Routes.Anarchy.StopAllActions)]
        public IActionResult StopAllActions()
        {
            _anarchyManager.StopAllActions();

            //TODO: When working return url to Get State of all AnarchyActions here...
            return new AcceptedResult();
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
    }
}