using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ActionStoppingExceptionHandler : BaseExceptionHandler<ActionStoppingException>
    {
        public ActionStoppingExceptionHandler(ILogAdapter<ActionStoppingException> logger) : base(logger, "action-stopping-request-aborted", StatusCodes.Status409Conflict)
        { }
    }
}
