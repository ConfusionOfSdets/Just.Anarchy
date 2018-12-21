using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class ScheduleExistsExceptionHandlerTests : BaseExceptionHandlerTests<ScheduleExistsExceptionHandler, ScheduleExistsException>
    {
        public ScheduleExistsExceptionHandlerTests() : base(() => new ScheduleExistsException())
        {
            ExpectedExceptionMessage = "The ActionOrchestrator already has a schedule, try to update the schedule instead.";
            ExpectedLogMessage = "schedule-already-exists - The ActionOrchestrator already has a schedule, try to update the schedule instead.";
            ExpectedCode = "schedule-already-exists";
        }
    }
}

