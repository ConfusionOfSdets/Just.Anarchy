using Microsoft.AspNetCore.Mvc;

namespace Just.Anarchy.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IAnarchyManagerNew _anarchyManager;

        public ScheduleController(IAnarchyManagerNew anarchyManager)
        {
            _anarchyManager = anarchyManager;
        }

        [HttpPost, Route(Routes.GetSetSchedule)]
        public IActionResult SetSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            _anarchyManager.AssignScheduleToAnarchyActionFactory(anarchyType, schedule, false);
            return new CreatedResult(GetFullUrl(Routes.GetSetSchedule.Replace("{anarchyType}", anarchyType)), schedule);
        }

        [HttpPut, Route(Routes.GetSetSchedule)]
        public IActionResult UpdateSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            var scheduleWasCreated = _anarchyManager.AssignScheduleToAnarchyActionFactory(anarchyType, schedule, true);
            return scheduleWasCreated ?
                (IActionResult) new CreatedResult(GetFullUrl(Routes.GetSetSchedule.Replace("{anarchyType}",anarchyType)), schedule) :
                new OkObjectResult(schedule);
        }

        [HttpGet, Route(Routes.GetSetSchedule)]
        public IActionResult GetSchedule(string anarchyType)
        {
            var schedule = _anarchyManager.GetScheduleFromAnarchyActionFactory(anarchyType);

            if (schedule == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(schedule);
        }

        private string GetFullUrl(string path) => $"{(this.Request.IsHttps ? "https://" : "http://")}{this.Request.Host}{path}";
    }
}