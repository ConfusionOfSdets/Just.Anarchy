using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.ActionOrchestratorTests
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
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/jim").Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, timer);
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
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/bob").Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, timer);
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
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath(url).Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, timer);
            sut.ForTargetPattern(null);

            //Act
            await sut.HandleRequest(context, next);

            //Assert
            await action.DidNotReceive().HandleRequestAsync(context, next, Arg.Any<CancellationToken>()); ;
        }

        [Test]
        public void HandleRequestErrorsWhenActionOrchestratorIsStopping()
        {
            //Arrange
            var ctsFromTest = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var timer = Substitute.For<IHandleTime>();
            var next = Substitute.For<RequestDelegate>();
            var initialExecution = true;

            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatHandlesRequestWithTask(async ctFromOrchestrator =>
                {
                    // the goal of this is to block the action execution on the first call,
                    // this will lead to an active task in _executionInstances that will need cancelling
                    if (initialExecution)
                    {
                        initialExecution = false;
                        await Block.UntilCancelled(ctsFromTest.Token);
                    }
                })
                .Build();
            
            var context = Get.CustomBuilderFor.MockHttpContext.WithPath("/bob").Build();
            
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, timer);
            sut.ForTargetPattern(".*");

#pragma warning disable 4014 // explicitly not awaiting here as we need to set separate tasks running that are blocked to trigger test state
            sut.HandleRequest(context, next);
            sut.Stop();
#pragma warning restore 4014

            //Act
            var exception = Assert.CatchAsync(async () => await sut.HandleRequest(context, next));
            ctsFromTest.Cancel();

            //Assert
            exception.Should().BeOfType<ActionStoppingException>();
        }
    }
}
