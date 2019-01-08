using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ScheduleRunningExceptionHandler : BaseExceptionHandler<ScheduleRunningException>
    {
        public ScheduleRunningExceptionHandler(ILogAdapter<ScheduleRunningException> logger) : base(logger, "schedule-running", StatusCodes.Status400BadRequest)
        { }
    }
}
