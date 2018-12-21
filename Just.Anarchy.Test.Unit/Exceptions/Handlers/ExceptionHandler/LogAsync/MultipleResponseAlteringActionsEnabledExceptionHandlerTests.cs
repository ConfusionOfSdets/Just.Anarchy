using System;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using Just.Anarchy.Test.Common.Fakes;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class MultipleResponseAlteringActionsEnabledExceptionHandlerTests : BaseExceptionHandlerTests<MultipleResponseAlteringActionsEnabledExceptionHandler, MultipleResponseAlteringActionsEnabledException>
    {
        public MultipleResponseAlteringActionsEnabledExceptionHandlerTests() : base(() => new MultipleResponseAlteringActionsEnabledException(new []{new FakeAnarchyAction()}))
        {
            ExpectedExceptionMessage = "There is more than 1 AnarchyAction enabled that is of type AlterResponse, this is not supported.";
            ExpectedLogMessage = "too-many-response-altering-anarchy-actions-enabled - There is more than 1 AnarchyAction enabled that is of type AlterResponse, this is not supported.";
            ExpectedCode = "too-many-response-altering-anarchy-actions-enabled";
        }
    }
}

