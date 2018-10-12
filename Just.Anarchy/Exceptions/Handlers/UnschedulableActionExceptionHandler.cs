using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class UnschedulableActionExceptionHandler : BaseExceptionHandler<UnschedulableActionException>
    {
        public UnschedulableActionExceptionHandler() : base("unschedulable-action-type", StatusCodes.Status400BadRequest)
        { }
    }
}
