using Just.Anarchy.Requests;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class SetActionTargetPatternRequestBodyRequiredExceptionHandler : 
        BaseExceptionHandler<RequestBodyRequiredException<EnableOnRequestHandlingRequest>>
    {
        private const string RequestType = "set-on-request-handling";

        public SetActionTargetPatternRequestBodyRequiredExceptionHandler() : 
            base($"{RequestType}-request-body-empty", StatusCodes.Status400BadRequest)
        { }
    }
}
