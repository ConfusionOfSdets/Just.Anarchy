using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ScheduleExistsExceptionHandler : BaseExceptionHandler<ScheduleExistsException>
    {
        public ScheduleExistsExceptionHandler() : base("schedule-already-exists", StatusCodes.Status400BadRequest)
        { }
    }
}
