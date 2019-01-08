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
    public class CanHandleTests
    {
        private static readonly object[] CanHandlePositiveCases =
        {
            new object[] { new ActionStoppingExceptionHandler(new FakeLogger<ActionStoppingException>()), new ActionStoppingException() },
            new object[] { new AnarchyActionNotFoundExceptionHandler(new FakeLogger<AnarchyActionNotFoundException>()), new AnarchyActionNotFoundException() },
            new object[] { new EmptyTargetPatternExceptionHandler(new FakeLogger<EmptyTargetPatternException>()), new EmptyTargetPatternException() },
            new object[] { new InvalidActionPayloadExceptionHandler(new FakeLogger<InvalidActionPayloadException>()), new InvalidActionPayloadException(typeof(FakeAnarchyAction), new JsonReaderException("this is a test")) },
            new object[] { new InvalidTargetPatternExceptionHandler(new FakeLogger<InvalidTargetPatternException>()), new InvalidTargetPatternException("test target pattern", new Exception("fake inner exception")) },
            new object[] { new MultipleResponseAlteringActionsEnabledExceptionHandler(new FakeLogger<MultipleResponseAlteringActionsEnabledException>()), new MultipleResponseAlteringActionsEnabledException(new [] {new FakeAnarchyAction() }) },
            new object[] { new ScheduleExistsExceptionHandler(new FakeLogger<ScheduleExistsException>()), new ScheduleExistsException() },
            new object[] { new ScheduleMissingExceptionHandler(new FakeLogger<ScheduleMissingException>()), new ScheduleMissingException() },
            new object[] { new ScheduleRunningExceptionHandler(new FakeLogger<ScheduleRunningException>()), new ScheduleRunningException()},
            new object[] { new SetActionTargetPatternRequestBodyRequiredExceptionHandler(new FakeLogger<RequestBodyRequiredException<EnableOnRequestHandlingRequest>>()), new RequestBodyRequiredException<EnableOnRequestHandlingRequest>() },
            new object[] { new UnschedulableActionExceptionHandler(new FakeLogger<UnschedulableActionException>()), new UnschedulableActionException()},
        };


        private static readonly object[] CanHandleNegativeCases =
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

        [TestCaseSource(nameof(CanHandlePositiveCases))]
        [Test]
        public void CallingCanHandleWithHandleableException(IExceptionHandler sut, Exception exception)
        {
            //Act
            var result = sut.CanHandle(exception);
            //Assert
            result.Should().BeTrue();
        }

        [TestCaseSource(nameof(CanHandleNegativeCases))]
        [Test]
        public void CallingCanHandleWithNonHandleableException(IExceptionHandler sut, Exception exception)
        {
            //Act
            var result = sut.CanHandle(exception);
            //Assert
            result.Should().BeFalse();
        }
    }
}

