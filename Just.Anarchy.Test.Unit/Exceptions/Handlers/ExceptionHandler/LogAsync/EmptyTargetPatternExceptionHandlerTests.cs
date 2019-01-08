using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class EmptyTargetPatternExceptionHandlerTests : BaseExceptionHandlerTests<EmptyTargetPatternExceptionHandler, EmptyTargetPatternException>
    {
        public EmptyTargetPatternExceptionHandlerTests() : base(() => new EmptyTargetPatternException())
        {
            ExpectedExceptionMessage = "The specified target pattern is empty.";
            ExpectedLogMessage = "empty-target-pattern-specified - The specified target pattern is empty.";
            ExpectedCode = "empty-target-pattern-specified";
        }
    }
}

