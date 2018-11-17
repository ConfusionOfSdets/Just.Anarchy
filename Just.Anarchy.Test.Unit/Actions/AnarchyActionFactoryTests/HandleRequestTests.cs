using System;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders.CustomBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.AnarchyActionFactoryTests
{
    [TestFixture]
    public class HandleRequestTests
    {
        [Test]
        public async Task HandleRequestRejectsIfTargetPatternDoesNotMatch()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var context = new FakeHttpContextBuilder().WithPath("/jim").Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern("/bob$");

            //Act
            await sut.HandleRequest(context, next);

            //Assert
            await action.DidNotReceive().HandleRequestAsync(context, next, Arg.Any<CancellationToken>()); ;
        }

        [Test]
        public async Task HandleRequestHandlesIfTargetPatternMatches()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var context = new FakeHttpContextBuilder().WithPath("/bob").Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern("/bob$");

            //Act
            await sut.HandleRequest(context, next);

            //Assert
            await action.Received(1).HandleRequestAsync(context, next, Arg.Any<CancellationToken>()); ;
        }

        [Test]
        [TestCase("/")]
        [TestCase("/bob")]
        [TestCase("")]
        public async Task HandleRequestRejectsAllUrlsIfTargetPatternIsNull(string url)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var context = new FakeHttpContextBuilder().WithPath(url).Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern(null);

            //Act
            await sut.HandleRequest(context, next);

            //Assert
            await action.DidNotReceive().HandleRequestAsync(context, next, Arg.Any<CancellationToken>()); ;
        }
    }
}
