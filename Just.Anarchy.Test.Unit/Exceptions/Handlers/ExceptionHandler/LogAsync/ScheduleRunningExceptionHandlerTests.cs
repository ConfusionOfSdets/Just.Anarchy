using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class ScheduleRunningExceptionHandlerTests : BaseExceptionHandlerTests<ScheduleRunningExceptionHandler, ScheduleRunningException>
    {
        public ScheduleRunningExceptionHandlerTests() : base(() => new ScheduleRunningException())
        {
            ExpectedExceptionMessage = "The ActionOrchestrator is active and running a schedule, stop the ActionOrchestrator before setting a new schedule.";
            ExpectedLogMessage = "schedule-running - The ActionOrchestrator is active and running a schedule, stop the ActionOrchestrator before setting a new schedule.";
            ExpectedCode = "schedule-running";
        }
    }
}

