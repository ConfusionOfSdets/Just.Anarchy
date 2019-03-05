using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.RandomErrorResponseAnarchy
{
    [TestFixture]
    public class HandleRequestAsyncTests
    {
        [Test]
        public async Task DoesNotCallNext()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var sut = new Anarchy.Actions.RandomErrorResponseAnarchy
            {
                StatusCode = 400,
                Body = "test body response"
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            await next.DidNotReceive().Invoke(context);
        }

        [Test]
        public async Task AltersHttpResponse_SetsBodyAsSpecifiedIfSet()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var sut = new Anarchy.Actions.RandomErrorResponseAnarchy
            {
                StatusCode = 400,
                Body = "test body response"
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            await context.Response.Body.ReceivedWithAnyArgs(1).WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task AltersHttpResponse_SetsBodyAsRandomStringIfNotSet()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var sut = new Anarchy.Actions.RandomErrorResponseAnarchy
            {
                StatusCode = 400,
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            var calls = context.Response.Body.Received(1).WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Is<int>(a => a > 0), ctFromTest);
        }

        [Test]
        public async Task AltersHttpResponse_SetsStatusCodeAsSpecifiedIfSet()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var sut = new Anarchy.Actions.RandomErrorResponseAnarchy
            {
                StatusCode = 400,
                Body = "test body response"
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            context.Response.StatusCode.Should().Be(sut.StatusCode);
        }

        [Test]
        public async Task AltersHttpResponse_SetsStatusCodeToRandomValidCodeIfNotSet()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var sut = new Anarchy.Actions.RandomErrorResponseAnarchy();
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            context.Response.StatusCode.Should().BeInRange(400,600);
        }
    }
}
