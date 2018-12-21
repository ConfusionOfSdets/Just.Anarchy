using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class UnschedulableActionExceptionHandlerTests : BaseExceptionHandlerTests<UnschedulableActionExceptionHandler, UnschedulableActionException>
    {
        public UnschedulableActionExceptionHandlerTests() : base(() => new UnschedulableActionException())
        {
            ExpectedExceptionMessage = "The ActionOrchestrator action specified is not able to be scheduled.";
            ExpectedLogMessage = "unschedulable-action-type - The ActionOrchestrator action specified is not able to be scheduled.";
            ExpectedCode = "unschedulable-action-type";
        }
    }
}

