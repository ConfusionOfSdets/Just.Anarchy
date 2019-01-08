using System;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class InvalidTargetPatternExceptionHandlerTests : BaseExceptionHandlerTests<InvalidTargetPatternExceptionHandler, InvalidTargetPatternException>
    {
        public InvalidTargetPatternExceptionHandlerTests() : base(() => new InvalidTargetPatternException("a fake target pattern", new ArgumentException()))
        {
            ExpectedExceptionMessage = "The specified target pattern is invalid, it needs to be a valid.net regular expression.";
            ExpectedLogMessage = "invalid-target-pattern-specified - The specified target pattern is invalid, it needs to be a valid.net regular expression.";
            ExpectedCode = "invalid-target-pattern-specified";
        }
    }
}

