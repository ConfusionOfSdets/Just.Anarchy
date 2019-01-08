using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class MultipleResponseAlteringActionsEnabledExceptionHandler : BaseExceptionHandler<MultipleResponseAlteringActionsEnabledException>
    {
        public MultipleResponseAlteringActionsEnabledExceptionHandler(
            ILogAdapter<MultipleResponseAlteringActionsEnabledException> logger) : 
            base(logger, "too-many-response-altering-anarchy-actions-enabled", StatusCodes.Status400BadRequest)
        { }
    }
}
