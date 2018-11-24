using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class HandleRequestTests
    {
        [Test]
        [TestCase(CauseAnarchyType.Passive)]
        [TestCase(CauseAnarchyType.AlterResponse)]
        public async Task MatchingOrchestrator(CauseAnarchyType causeAnarchyType)
        {
            //arrange
            var orchestrator = GetMockOrchestratorWithAnarchyTypeAndHandlingState(true, causeAnarchyType);
                
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            await sut.HandleRequest(context, next);

            //assert
            await orchestrator.Received(1).HandleRequest(context, next);
        }

        [Test]
        [TestCase(CauseAnarchyType.Passive)]
        [TestCase(CauseAnarchyType.AlterResponse)]
        public async Task NoMatchingOrchestrator(CauseAnarchyType causeAnarchyType)
        {
            //arrange
            var orchestrator = GetMockOrchestratorWithAnarchyTypeAndHandlingState(false, causeAnarchyType);

            var sut = new AnarchyManagerNew(new[] { orchestrator });

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            await sut.HandleRequest(context, next);

            //assert
            await orchestrator.Received(0).HandleRequest(context, next);
        }

        [Test]
        [TestCase(CauseAnarchyType.Passive, CauseAnarchyType.Passive, null)]
        [TestCase(CauseAnarchyType.Passive, null, CauseAnarchyType.Passive)]
        [TestCase(CauseAnarchyType.AlterResponse, null, null)]
        [TestCase(null, CauseAnarchyType.AlterResponse, null)]
        [TestCase(null, CauseAnarchyType.AlterResponse, CauseAnarchyType.Passive)]
        public async Task CallsAllMatchingOrchestrators(CauseAnarchyType? firstCanHandle, CauseAnarchyType? secondCanHandle, CauseAnarchyType? thirdCanHandle)
        {
            //arrange
            var canHandleStates = new[] {firstCanHandle, secondCanHandle, thirdCanHandle};

            var orchestrators = canHandleStates.Select(state =>
                GetMockOrchestratorWithAnarchyTypeAndHandlingState(state != null, state ?? CauseAnarchyType.Passive))
                .ToList();

            var sut = new AnarchyManagerNew(orchestrators);

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            await sut.HandleRequest(context, next);

            //assert
            for (var i = 0; i < canHandleStates.Length; i++)
            {
                var shouldHaveHandled = canHandleStates[i] != null;
                if (shouldHaveHandled)
                {
                    await orchestrators[i].Received(1)
                        .HandleRequest(context, next);
                }
                else
                {
                    await orchestrators[i].DidNotReceive()
                        .HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>());
                }
            }
        }

        [Test]
        public void OnlyAllowsASingleHandlingActionOrchestratorOfTypeAlterResponse()
        {
            //arrange

            var actionOrchestrators = new[]
            {
                GetMockOrchestratorWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.AlterResponse),
                GetMockOrchestratorWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.Passive),
                GetMockOrchestratorWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.AlterResponse)
            };
        
            var sut = new AnarchyManagerNew(actionOrchestrators);

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            var exception = Assert.CatchAsync(async () => await sut.HandleRequest(context, next));

            //assert
            exception.Should().BeOfType<MultipleResponseAlteringActionsEnabledException>();
        }

        [Test]
        public async Task AlwaysCallsAlterResponseOrchestratorLast()
        {
            //arrange
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            DateTime? alterResponseCalledAt = null;
            DateTime? passiveResponseCalledAt = null;

            var passiveOrchestrator = GetMockOrchestratorWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.Passive);
                passiveOrchestrator.HandleRequest(context, next)
                .Returns(Task.Delay(100))
                .AndDoes(_ => passiveResponseCalledAt = DateTime.Now);

            var alterResponseOrchestrator = GetMockOrchestratorWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.AlterResponse);
            alterResponseOrchestrator.HandleRequest(context, next)
                .Returns(Task.Delay(100))
                .AndDoes(_ => alterResponseCalledAt = DateTime.Now);

            var sut = new AnarchyManagerNew(new [] { alterResponseOrchestrator, passiveOrchestrator });

            //act
            await sut.HandleRequest(context, next);

            //assert
            alterResponseCalledAt.Value.Should().BeAfter(passiveResponseCalledAt.Value);
        }

        private IActionOrchestrator GetMockOrchestratorWithAnarchyTypeAndHandlingState(bool canHandle, CauseAnarchyType anarchyType)
        {
            var action = Get.CustomBuilderFor
                .MockAnarchyAction
                .WithCauseAnarchyType(anarchyType)
                .Build();

            var orchestrator = Get.CustomBuilderFor
                .MockAnarchyActionOrchestrator
                .WithAction(action)
                .ThatHasCanHandleResponse(canHandle);

            return orchestrator.Build();
        }
    }
}
