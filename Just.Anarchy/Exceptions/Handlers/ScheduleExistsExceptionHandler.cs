using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ScheduleExistsExceptionHandler : BaseExceptionHandler<ScheduleExistsException>
    {
        public ScheduleExistsExceptionHandler(ILogAdapter<ScheduleExistsException> logger) : base(logger, "schedule-already-exists", StatusCodes.Status400BadRequest)
        { }
    }
}
