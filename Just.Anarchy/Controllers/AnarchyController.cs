using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy.Controllers
{
    public class AnarchyController : Controller
    {
        private readonly IAnarchyManagerNew _anarchyManager;
        private readonly ILogger<AnarchyController> _logger;

        public AnarchyController(IAnarchyManagerNew anarchyManager, ILogger<AnarchyController> logger)
        {
            _anarchyManager = anarchyManager;
            _logger = logger;
        }

        [HttpPost, Route(Routes.Anarchy.SetOrCancelOnRequestHandling)]
        public IActionResult SetActionTargetPattern(string anarchyType, [FromBody]EnableOnRequestHandlingRequest request)
        {
            _logger.LogInformation("Assigning target pattern: '{targetPattern}' to {anarchyType}", request?.TargetPattern ?? "null", anarchyType);
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
            var durationMessage = durationSecs.HasValue ? $" with duration {durationSecs.Value}s" : null;
            _logger.LogInformation("Triggering anarchy action '{anarchyType}'{durationMessage}.", anarchyType, durationMessage);
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

            _logger.LogInformation("Updating action properties for anarchy action '{anarchyType} with properties: '{updatedAction}'.", anarchyType, updatedAnarchyJson);

            _anarchyManager.UpdateAction(anarchyType, updatedAnarchyJson);
            return new OkResult();
        }

        [HttpPut, Route(Routes.Anarchy.StopAction)]
        public IActionResult StopAction(string anarchyType)
        {
            _logger.LogInformation("Stopping any running executions on anarchy action '{anarchyType}'.", anarchyType);
            _anarchyManager.StopAction(anarchyType);

            //TODO: When working return url to Get State of AnarchyAction here...
            return new AcceptedResult();
        }

        [HttpPut, Route(Routes.Anarchy.StopAllActions)]
        public IActionResult StopAllActions()
        {
            _logger.LogInformation("Stopping any running executions on all anarchy actions.");
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