using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ScheduleMissingExceptionHandler : BaseExceptionHandler<ScheduleMissingException>
    {
        public ScheduleMissingExceptionHandler(ILogAdapter<ScheduleMissingException> logger) : base(logger, "schedule-not-set", StatusCodes.Status400BadRequest)
        { }
    }
}
