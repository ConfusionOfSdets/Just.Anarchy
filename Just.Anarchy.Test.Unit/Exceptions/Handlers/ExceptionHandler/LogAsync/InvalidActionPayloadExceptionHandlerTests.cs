using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using Just.Anarchy.Test.Common.Fakes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    [TestFixture]
    public class InvalidActionPayloadExceptionHandlerTests : BaseExceptionHandlerTests<InvalidActionPayloadExceptionHandler, InvalidActionPayloadException>
    {
        public InvalidActionPayloadExceptionHandlerTests() : base(() => new InvalidActionPayloadException(typeof(FakeAnarchyAction), new JsonReaderException("this is a test")))
        {
            ExpectedExceptionMessage = "The specified json payload is invalid, or a property listed in the payload is readonly.";
            ExpectedLogMessage = "invalid-action-payload - The specified json payload is invalid, or a property listed in the payload is readonly.";
            ExpectedCode = "invalid-action-payload";
        }
    }
}

