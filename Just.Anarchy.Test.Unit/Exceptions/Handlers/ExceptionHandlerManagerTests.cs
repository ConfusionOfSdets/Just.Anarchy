using System;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions.Handlers;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers
{
    [TestFixture]
    public class ExceptionHandlerManagerTests
    {
        [Test]
        public async Task HandleAsyncCallsHandler()
        {
            //Arrange
            var response = new DefaultHttpContext().Response;
            var handler = Substitute.For<IExceptionHandler>();
            handler.CanHandle(Arg.Any<Exception>()).Returns(true);
            handler
                .HandleExceptionAsync(Arg.Any<HttpResponse>(), Arg.Any<Exception>())
                .Returns(r => Task.CompletedTask);
            var exception = new ArgumentException();
            var sut = new ExceptionHandlerManager(new[] {handler});

            //Act
            await sut.HandleAsync(response, exception);

            //Assert
            await handler.Received(1).HandleExceptionAsync(response, exception);
        }

        [Test]
        public void HandleAsyncWithoutHandlersThrowsException()
        {
            //Arrange
            var response = new DefaultHttpContext().Response;
            var exception = new ArgumentNullException();
            var sut = new ExceptionHandlerManager(new IExceptionHandler[] {});

            //Act
            var result = Assert.CatchAsync(async () => await sut.HandleAsync(response, exception));

            //Assert
            result.Should().Be(exception);
        }

        [Test]
        public void HandleAsyncThrowsExceptionIfItCannotBeHandled()
        {
            //Arrange
            var response = new DefaultHttpContext().Response;
            var handler1 = Substitute.For<IExceptionHandler>();
            handler1.CanHandle(Arg.Any<Exception>()).Returns(false);
            var handler2 = Substitute.For<IExceptionHandler>();
            handler2.CanHandle(Arg.Any<Exception>()).Returns(false);
            var exception = new ArgumentNullException();
            var sut = new ExceptionHandlerManager(new[] { handler1, handler2 });

            //Act
            var result = Assert.CatchAsync(async () => await sut.HandleAsync(response, exception));

            //Assert
            result.Should().Be(exception);
        }

        [Test]
        public void HandleAsyncChecksAllHandlers()
        {
            //Arrange
            var response = new DefaultHttpContext().Response;
            var handler1 = Substitute.For<IExceptionHandler>();
            handler1.CanHandle(Arg.Any<Exception>()).Returns(false);
            var handler2 = Substitute.For<IExceptionHandler>();
            handler2.CanHandle(Arg.Any<Exception>()).Returns(false);
            var exception = new ArgumentNullException();
            var sut = new ExceptionHandlerManager(new[] { handler1, handler2 });

            //Act
            Assert.CatchAsync(async () => await sut.HandleAsync(response, exception));

            //Assert
            handler1.Received(1).CanHandle(exception);
            handler2.Received(1).CanHandle(exception);
        }
    }
}
