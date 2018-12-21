using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using Just.Anarchy.Requests;
using Just.Anarchy.Test.Common.Extensions;
using Just.Anarchy.Test.Common.Fakes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler
{
    [TestFixture]
    public class HandleAsyncTests
    {
        private static readonly object[] HandleAsyncFailingCases =
        {
            new object[] { new ActionStoppingExceptionHandler(new FakeLogger<ActionStoppingException>()), new ApplicationException() },
            new object[] { new AnarchyActionNotFoundExceptionHandler(new FakeLogger<AnarchyActionNotFoundException>()), new NullReferenceException() },
            new object[] { new EmptyTargetPatternExceptionHandler(new FakeLogger<EmptyTargetPatternException>()), new NullReferenceException() },
            new object[] { new InvalidActionPayloadExceptionHandler(new FakeLogger<InvalidActionPayloadException>()), new ActionStoppingException() },
            new object[] { new InvalidTargetPatternExceptionHandler(new FakeLogger<InvalidTargetPatternException>()), new AnarchyActionNotFoundException() },
            new object[] { new MultipleResponseAlteringActionsEnabledExceptionHandler(new FakeLogger<MultipleResponseAlteringActionsEnabledException>()), new NullReferenceException() },
            new object[] { new ScheduleExistsExceptionHandler(new FakeLogger<ScheduleExistsException>()), new ArgumentException() },
            new object[] { new ScheduleMissingExceptionHandler(new FakeLogger<ScheduleMissingException>()), new NullReferenceException() },
            new object[] { new ScheduleRunningExceptionHandler(new FakeLogger<ScheduleRunningException>()), new ArgumentException()},
            new object[] { new SetActionTargetPatternRequestBodyRequiredExceptionHandler(new FakeLogger<RequestBodyRequiredException<EnableOnRequestHandlingRequest>>()), new ApplicationException() },
            new object[] { new UnschedulableActionExceptionHandler(new FakeLogger<UnschedulableActionException>()), new ArgumentException()},
        };

        [TestCaseSource(nameof(HandleAsyncFailingCases))]
        [Test]
        public void CallingHandleWithNonHandleableTypeThrowsArgumentException(IExceptionHandler sut, Exception notHandledException)
        {
            //Arrange
            var testResponse = new DefaultHttpContext().Response;
            //Act
            var exception = Assert.CatchAsync(async () => await sut.HandleExceptionAsync(testResponse, notHandledException));
            //Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        private static readonly object[] HandleAsyncPassingCases =
        {
            new object[] { new ActionStoppingExceptionHandler(new FakeLogger<ActionStoppingException>()), new ActionStoppingException(), "action-stopping-request-aborted", StatusCodes.Status409Conflict },
            new object[] { new AnarchyActionNotFoundExceptionHandler(new FakeLogger<AnarchyActionNotFoundException>()), new AnarchyActionNotFoundException(), "anarchy-action-not-found", StatusCodes.Status404NotFound },
            new object[] { new EmptyTargetPatternExceptionHandler(new FakeLogger<EmptyTargetPatternException>()), new EmptyTargetPatternException(), "empty-target-pattern-specified", StatusCodes.Status400BadRequest },
            new object[] { new InvalidActionPayloadExceptionHandler(new FakeLogger<InvalidActionPayloadException>()), new InvalidActionPayloadException(typeof(FakeAnarchyAction), new JsonReaderException("this is a test")), "invalid-action-payload", StatusCodes.Status400BadRequest },
            new object[] { new InvalidTargetPatternExceptionHandler(new FakeLogger<InvalidTargetPatternException>()), new InvalidTargetPatternException("test target pattern", new Exception("fake inner exception")), "invalid-target-pattern-specified", StatusCodes.Status400BadRequest },
            new object[] { new MultipleResponseAlteringActionsEnabledExceptionHandler(new FakeLogger<MultipleResponseAlteringActionsEnabledException>()), new MultipleResponseAlteringActionsEnabledException(new [] {new FakeAnarchyAction() }), "too-many-response-altering-anarchy-actions-enabled", StatusCodes.Status400BadRequest },
            new object[] { new ScheduleExistsExceptionHandler(new FakeLogger<ScheduleExistsException>()), new ScheduleExistsException(), "schedule-already-exists", StatusCodes.Status400BadRequest },
            new object[] { new ScheduleMissingExceptionHandler(new FakeLogger<ScheduleMissingException>()), new ScheduleMissingException(), "schedule-missing", StatusCodes.Status400BadRequest },
            new object[] { new ScheduleRunningExceptionHandler(new FakeLogger<ScheduleRunningException>()), new ScheduleRunningException(), "schedule-running", StatusCodes.Status400BadRequest },
            new object[] { new SetActionTargetPatternRequestBodyRequiredExceptionHandler(new FakeLogger<RequestBodyRequiredException<EnableOnRequestHandlingRequest>>()), new RequestBodyRequiredException<EnableOnRequestHandlingRequest>(), "set-on-request-handling-request-body-empty", StatusCodes.Status400BadRequest },
            new object[] { new UnschedulableActionExceptionHandler(new FakeLogger<UnschedulableActionException>()), new UnschedulableActionException(), "unschedulable-action-type", StatusCodes.Status400BadRequest },
        };

        [TestCaseSource(nameof(HandleAsyncPassingCases))]
        [Test]
        public async Task CallingHandleWithHandleableTypeSetsResponseStatus(IExceptionHandler sut, Exception exception, string expectedErrorCode, int statusCode)
        {
            //Arrange
            var testResponse = new DefaultHttpContext().Response;

            //Act
            await sut.HandleExceptionAsync(testResponse, exception);

            //Assert
            testResponse.StatusCode.Should().Be(statusCode);
        }

        [TestCaseSource(nameof(HandleAsyncPassingCases))]
        [Test]
        public async Task CallingHandleWithHandleableTypeSetsResponseBody(IExceptionHandler sut, Exception exception, string expectedErrorCode, int statusCode)
        {
            //Arrange
            var testResponse = new DefaultHttpContext().Response;
            testResponse.Body = new MemoryStream();
            var expectedBody = (new ControllerErrorResult { Errors = new[] { expectedErrorCode } }).Serialise();

            //Act
            await sut.HandleExceptionAsync(testResponse, exception);
            var result = testResponse.Body.ConvertToString();

            //Assert
            result.Should().Be(expectedBody);
        }
    }
}

