using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ScheduleMissingExceptionHandler : BaseExceptionHandler<ScheduleMissingException>
    {
        public ScheduleMissingExceptionHandler() : base("schedule-not-set", StatusCodes.Status400BadRequest)
        { }
    }
}
