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

        [HttpPost, Route("/anarchy/schedule/{anarchyType}")]
        public IActionResult SetSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            _anarchyManager.AssignScheduleToAnarchyActionFactory(anarchyType, schedule);
            return new OkResult();
        }
    }
}