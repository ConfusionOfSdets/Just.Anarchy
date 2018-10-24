using System.Linq;
using Just.Anarchy.Exceptions;
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
            _anarchyManager.AssignScheduleToAnarchyActionFactory(anarchyType, schedule);
            return new OkResult();
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
    }
}