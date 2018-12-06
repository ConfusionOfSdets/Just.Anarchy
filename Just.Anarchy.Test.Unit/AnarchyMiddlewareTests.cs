using System;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit
{
    [TestFixture]
    public class AnarchyMiddlewareTests
    {
        [Test]
        public async Task Invoke_TriggersAnarchyManager_ForNonAnarchyRoutes()
        {
            // Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var exceptionHandler = Substitute.For<IHandleAnarchyExceptions>();
            var next = Substitute.For<RequestDelegate>();
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/foo").Build();
            var sut = new AnarchyMiddleware(next, anarchyManager, exceptionHandler);

            // Act
            await sut.Invoke(context);

            // Assert
            await anarchyManager.Received(1).HandleRequest(context, next);
        }

        [Test]
        public async Task Invoke_DoesNotTriggerAnarchyManager_ForAnarchyRoutes()
        {
            // Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var exceptionHandler = Substitute.For<IHandleAnarchyExceptions>();
            var next = Substitute.For<RequestDelegate>();
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/anarchy/foo").Build();
            var sut = new AnarchyMiddleware(next, anarchyManager, exceptionHandler);

            // Act
            await sut.Invoke(context);

            // Assert
            await anarchyManager.DidNotReceive().HandleRequest(context, next);
        }

        [Test]
        public async Task Invoke_CallsNext_ForNonAnarchyRoutes_ThatDoNotAffectResponse()
        {
            // Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>())
                .Returns(Task.FromResult(false));
            var exceptionHandler = Substitute.For<IHandleAnarchyExceptions>();
            var next = Substitute.For<RequestDelegate>();
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/foo").Build();
            var sut = new AnarchyMiddleware(next, anarchyManager, exceptionHandler);

            // Act
            await sut.Invoke(context);

            // Assert
            await next.Received(1).Invoke(context);
        }

        [Test]
        public async Task Invoke_DoesNotCallNext_ForNonAnarchyRoutes_ThatAffectResponse()
        {
            // Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>())
                .Returns(Task.FromResult(true));
            var exceptionHandler = Substitute.For<IHandleAnarchyExceptions>();
            var next = Substitute.For<RequestDelegate>();
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/foo").Build();
            var sut = new AnarchyMiddleware(next, anarchyManager, exceptionHandler);

            // Act
            await sut.Invoke(context);

            // Assert
            await next.DidNotReceive().Invoke(context);
        }

        [Test]
        public async Task Invoke_HandlesErrorFromManager()
        {
            // Arrange
            var testException = new ArgumentOutOfRangeException("this is a test exception");
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>())
                .Throws(testException);
            var exceptionHandler = Substitute.For<IHandleAnarchyExceptions>();
            var next = Substitute.For<RequestDelegate>();
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/bob").Build();
            var sut = new AnarchyMiddleware(next, anarchyManager, exceptionHandler);

            // Act
            await sut.Invoke(context);

            // Assert
            await exceptionHandler.Received(1).HandleAsync(context.Response, testException);
        }

        [Test]
        public async Task Invoke_HandlesErrorFromNextRequestDelegate()
        {
            // Arrange
            var testException = new ApplicationException("this is a test exception");
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var exceptionHandler = Substitute.For<IHandleAnarchyExceptions>();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(Arg.Any<HttpContext>()).Throws(testException);
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/anarchy/bob").Build();
            var sut = new AnarchyMiddleware(next, anarchyManager, exceptionHandler);

            // Act
            await sut.Invoke(context);

            // Assert
            await exceptionHandler.Received(1).HandleAsync(context.Response, testException);
        }
    }
}