using Just.Anarchy.Core;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Responses;
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

        [HttpPost, Route(Routes.Schedule.GetSetUpdate)]
        public IActionResult SetSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            _anarchyManager.AssignScheduleToAnarchyActionFactory(anarchyType, schedule, false);
            return new CreatedResult(GetFullUrl(Routes.Schedule.GetSetUpdate.Replace("{anarchyType}", anarchyType)), schedule);
        }

        [HttpPut, Route(Routes.Schedule.GetSetUpdate)]
        public IActionResult UpdateSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            var scheduleWasCreated = _anarchyManager.AssignScheduleToAnarchyActionFactory(anarchyType, schedule, true);
            return scheduleWasCreated ?
                (IActionResult) new CreatedResult(GetFullUrl(Routes.Schedule.GetSetUpdate.Replace("{anarchyType}",anarchyType)), schedule) :
                new OkObjectResult(schedule);
        }

        [HttpGet, Route(Routes.Schedule.GetSetUpdate)]
        public IActionResult GetSchedule(string anarchyType)
        {
            var schedule = _anarchyManager.GetScheduleFromAnarchyActionFactory(anarchyType);

            if (schedule == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(schedule);
        }

        [HttpGet, Route(Routes.Schedule.GetAll)]
        public IActionResult GetAllSchedules()
        {
            var schedules = new EnumerableResultResponse<NamedScheduleDto>(_anarchyManager.GetAllSchedulesFromFactories());

            return new OkObjectResult(schedules);
        }

        private string GetFullUrl(string path) => $"{(this.Request.IsHttps ? "https://" : "http://")}{this.Request.Host}{path}";
    }
}