using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using Just.Anarchy.Requests;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class SetActionTargetPatternRequestBodyRequiredExceptionHandlerTests : BaseExceptionHandlerTests<SetActionTargetPatternRequestBodyRequiredExceptionHandler, RequestBodyRequiredException<EnableOnRequestHandlingRequest>>
    {
        public SetActionTargetPatternRequestBodyRequiredExceptionHandlerTests() : base(() => new RequestBodyRequiredException<EnableOnRequestHandlingRequest>())
        {
            ExpectedExceptionMessage = "The specified endpoint must have a request body of type 'Just.Anarchy.Requests.EnableOnRequestHandlingRequest'.";
            ExpectedLogMessage = "set-on-request-handling-request-body-empty - The specified endpoint must have a request body of type 'Just.Anarchy.Requests.EnableOnRequestHandlingRequest'.";
            ExpectedCode = "set-on-request-handling-request-body-empty";
        }
    }
}

