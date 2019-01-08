using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class ActionStoppingExceptionHandlerTests : BaseExceptionHandlerTests<ActionStoppingExceptionHandler , ActionStoppingException>
    {
        public ActionStoppingExceptionHandlerTests() : base(() => new ActionStoppingException())
        {
            ExpectedExceptionMessage = "The ActionOrchestrator has been requested to stop, your request cannot be fulfilled.";
            ExpectedLogMessage = "action-stopping-request-aborted - The ActionOrchestrator has been requested to stop, your request cannot be fulfilled.";
            ExpectedCode = "action-stopping-request-aborted";
        }
    }
}

