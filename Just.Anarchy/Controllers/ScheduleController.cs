using Just.Anarchy.Core;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Just.Anarchy.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IAnarchyManagerNew _anarchyManager;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IAnarchyManagerNew anarchyManager, ILogger<ScheduleController> logger)
        {
            _anarchyManager = anarchyManager;
            _logger = logger;
        }

        [HttpPost, Route(Routes.Schedule.GetSetUpdate)]
        public IActionResult SetSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            _logger.LogInformation("Assigning Schedule to '{anarchyType}' ({schedule})", anarchyType, JsonConvert.SerializeObject(schedule));
            _anarchyManager.AssignScheduleToActionOrchestrator(anarchyType, schedule, false);
            return new CreatedResult(GetFullUrl(Routes.Schedule.GetSetUpdate.Replace("{anarchyType}", anarchyType)), schedule);
        }

        [HttpPut, Route(Routes.Schedule.GetSetUpdate)]
        public IActionResult UpdateSchedule(string anarchyType, [FromBody]Schedule schedule)
        {
            _logger.LogInformation("Updating schedule for anarchy action '{anarchyType}' with schedule: ({schedule})", anarchyType, JsonConvert.SerializeObject(schedule));
            var scheduleWasCreated = _anarchyManager.AssignScheduleToActionOrchestrator(anarchyType, schedule, true);
            return scheduleWasCreated ?
                (IActionResult) new CreatedResult(GetFullUrl(Routes.Schedule.GetSetUpdate.Replace("{anarchyType}",anarchyType)), schedule) :
                new OkObjectResult(schedule);
        }

        [HttpGet, Route(Routes.Schedule.GetSetUpdate)]
        public IActionResult GetSchedule(string anarchyType)
        {
            _logger.LogInformation("Retrieving schedule for anarchy action '{anarchyType}'", anarchyType);
            var schedule = _anarchyManager.GetScheduleFromActionOrchestrator(anarchyType);

            if (schedule == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(schedule);
        }

        [HttpGet, Route(Routes.Schedule.GetAll)]
        public IActionResult GetAllSchedules()
        {
            _logger.LogInformation("Retrieving schedule for all schedulable anarchy actions");

            var schedules = new EnumerableResultResponse<NamedScheduleDto>(_anarchyManager.GetAllSchedulesFromOrchestrators());

            return new OkObjectResult(schedules);
        }

        [HttpPut, Route(Routes.Schedule.Start)]
        public IActionResult StartSchedule(string anarchyType)
        {
            _logger.LogInformation("Starting schedule for anarchy action '{anarchyType}'", anarchyType);
            _anarchyManager.StartSchedule(anarchyType);

            return new OkResult();
        }

        [HttpPut, Route(Routes.Schedule.StartAll)]
        public IActionResult StartAllSchedules()
        {
            _logger.LogInformation("Attempting to start all schedulable anarchy actions");
            _anarchyManager.StartAllSchedules();
            return new AcceptedResult();
        }

        private string GetFullUrl(string path) => $"{(this.Request.IsHttps ? "https://" : "http://")}{this.Request.Host}{path}";
    }
}