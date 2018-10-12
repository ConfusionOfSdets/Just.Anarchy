using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using Just.Anarchy.Test.Unit.utils;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers
{
    [TestFixture]
    public class BaseExceptionHandlerTests
    {
        private static readonly object[] CanHandleCases =
        {
            new object[] { new ScheduleRunningExceptionHandler(), new ScheduleRunningException(), true },
            new object[] { new ScheduleRunningExceptionHandler(), new ArgumentException(), false },
            new object[] { new UnschedulableActionExceptionHandler(), new UnschedulableActionException(), true },
            new object[] { new UnschedulableActionExceptionHandler(), new ArgumentException(), false }
        };

        [TestCaseSource(nameof(CanHandleCases))]
        [Test]
        public void CallingCanHandle(IExceptionHandler sut, Exception exception, bool expResult)
        {
            //Act
            var result = sut.CanHandle(exception);
            //Assert
            result.Should().Be(expResult);
        }

        private static readonly object[] HandleAsyncFailingCases =
        {
            new object[] { new ScheduleRunningExceptionHandler(), new ArgumentException() },
            new object[] { new UnschedulableActionExceptionHandler(), new ArgumentException() }
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
            new object[] { new ScheduleRunningExceptionHandler(), new ScheduleRunningException(), "schedule-running", StatusCodes.Status400BadRequest },
            new object[] { new UnschedulableActionExceptionHandler(), new UnschedulableActionException(), "unschedulable-action-type", StatusCodes.Status400BadRequest }
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
