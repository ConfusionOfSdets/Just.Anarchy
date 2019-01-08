using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class AnarchyActionNotFoundExceptionHandlerTests : BaseExceptionHandlerTests<AnarchyActionNotFoundExceptionHandler, AnarchyActionNotFoundException>
    {
        public AnarchyActionNotFoundExceptionHandlerTests() : base(() => new AnarchyActionNotFoundException())
        {
            ExpectedExceptionMessage = "The specified anarchy action does not exist.";
            ExpectedLogMessage = "anarchy-action-not-found - The specified anarchy action does not exist.";
            ExpectedCode = "anarchy-action-not-found";
        }
    }
}

