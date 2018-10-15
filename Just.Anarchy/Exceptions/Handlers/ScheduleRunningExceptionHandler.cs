using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ScheduleRunningExceptionHandler : BaseExceptionHandler<ScheduleRunningException>
    {
        public ScheduleRunningExceptionHandler() : base("schedule-running", StatusCodes.Status400BadRequest)
        { }
    }
}
