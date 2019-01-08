using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class UnschedulableActionExceptionHandler : BaseExceptionHandler<UnschedulableActionException>
    {
        public UnschedulableActionExceptionHandler(ILogAdapter<UnschedulableActionException> logger) : 
            base(logger, "unschedulable-action-type", StatusCodes.Status400BadRequest)
        { }
    }
}
