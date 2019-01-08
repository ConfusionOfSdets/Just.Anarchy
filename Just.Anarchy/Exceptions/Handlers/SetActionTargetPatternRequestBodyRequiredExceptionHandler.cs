using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Requests;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class SetActionTargetPatternRequestBodyRequiredExceptionHandler : 
        BaseExceptionHandler<RequestBodyRequiredException<EnableOnRequestHandlingRequest>>
    {
        public SetActionTargetPatternRequestBodyRequiredExceptionHandler(
            ILogAdapter<RequestBodyRequiredException<EnableOnRequestHandlingRequest>> logger) : 
            base(logger, "set-on-request-handling-request-body-empty", StatusCodes.Status400BadRequest)
        { }
    }
}
