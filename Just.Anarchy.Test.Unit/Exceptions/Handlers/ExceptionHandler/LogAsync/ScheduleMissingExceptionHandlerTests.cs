using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class ScheduleMissingExceptionHandlerTests : BaseExceptionHandlerTests<ScheduleMissingExceptionHandler, ScheduleMissingException>
    {
        public ScheduleMissingExceptionHandlerTests() : base(() => new ScheduleMissingException())
        {
            ExpectedExceptionMessage = "The ActionOrchestrator does not have a schedule set, this needs to be specified first.";
            ExpectedLogMessage = "schedule-not-set - The ActionOrchestrator does not have a schedule set, this needs to be specified first.";
            ExpectedCode = "schedule-not-set";
        }
    }
}

