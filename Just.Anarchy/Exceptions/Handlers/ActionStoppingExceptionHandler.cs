using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ActionStoppingExceptionHandler : BaseExceptionHandler<ActionStoppingException>
    {
        public ActionStoppingExceptionHandler() : base("action-stopping-request-aborted", StatusCodes.Status409Conflict)
        { }
    }
}
