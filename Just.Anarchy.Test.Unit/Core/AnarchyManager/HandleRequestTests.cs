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
        public async Task MatchingFactory(CauseAnarchyType causeAnarchyType)
        {
            //arrange
            var factory = GetMockFactoryWithAnarchyTypeAndHandlingState(true, causeAnarchyType);
                
            var sut = new AnarchyManagerNew(new [] { factory });

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            await sut.HandleRequest(context, next);

            //assert
            await factory.Received(1).HandleRequest(context, next);
        }

        [Test]
        [TestCase(CauseAnarchyType.Passive)]
        [TestCase(CauseAnarchyType.AlterResponse)]
        public async Task NoMatchingFactory(CauseAnarchyType causeAnarchyType)
        {
            //arrange
            var factory = GetMockFactoryWithAnarchyTypeAndHandlingState(false, causeAnarchyType);

            var sut = new AnarchyManagerNew(new[] { factory });

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            await sut.HandleRequest(context, next);

            //assert
            await factory.Received(0).HandleRequest(context, next);
        }

        [Test]
        [TestCase(CauseAnarchyType.Passive, CauseAnarchyType.Passive, null)]
        [TestCase(CauseAnarchyType.Passive, null, CauseAnarchyType.Passive)]
        [TestCase(CauseAnarchyType.AlterResponse, null, null)]
        [TestCase(null, CauseAnarchyType.AlterResponse, null)]
        [TestCase(null, CauseAnarchyType.AlterResponse, CauseAnarchyType.Passive)]
        public async Task CallsAllMatchingFactories(CauseAnarchyType? firstCanHandle, CauseAnarchyType? secondCanHandle, CauseAnarchyType? thirdCanHandle)
        {
            //arrange
            var canHandleStates = new[] {firstCanHandle, secondCanHandle, thirdCanHandle};

            var factories = canHandleStates.Select(state =>
                GetMockFactoryWithAnarchyTypeAndHandlingState(state != null, state ?? CauseAnarchyType.Passive))
                .ToList();

            var sut = new AnarchyManagerNew(factories);

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
                    await factories[i].Received(1)
                        .HandleRequest(context, next);
                }
                else
                {
                    await factories[i].DidNotReceive()
                        .HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>());
                }
            }
        }

        [Test]
        public void OnlyAllowsASingleHandlingFactoryOfTypeAlterResponse()
        {
            //arrange

            var factories = new[]
            {
                GetMockFactoryWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.AlterResponse),
                GetMockFactoryWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.Passive),
                GetMockFactoryWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.AlterResponse)
            };
        
            var sut = new AnarchyManagerNew(factories);

            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            //act
            var exception = Assert.CatchAsync(async () => await sut.HandleRequest(context, next));

            //assert
            exception.Should().BeOfType<MultipleResponseAlteringActionsEnabledException>();
        }

        [Test]
        public async Task AlwaysCallsAlterResponseFactoryLast()
        {
            //arrange
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();

            DateTime? alterResponseCalledAt = null;
            DateTime? passiveResponseCalledAt = null;

            var passiveFactory = GetMockFactoryWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.Passive);
                passiveFactory.HandleRequest(context, next)
                .Returns(Task.Delay(100))
                .AndDoes(_ => passiveResponseCalledAt = DateTime.Now);

            var alterResponseFactory = GetMockFactoryWithAnarchyTypeAndHandlingState(true, CauseAnarchyType.AlterResponse);
            alterResponseFactory.HandleRequest(context, next)
                .Returns(Task.Delay(100))
                .AndDoes(_ => alterResponseCalledAt = DateTime.Now);

            var sut = new AnarchyManagerNew(new [] { alterResponseFactory, passiveFactory });

            //act
            await sut.HandleRequest(context, next);

            //assert
            alterResponseCalledAt.Value.Should().BeAfter(passiveResponseCalledAt.Value);
        }

        private IAnarchyActionFactory GetMockFactoryWithAnarchyTypeAndHandlingState(bool canHandle, CauseAnarchyType anarchyType)
        {
            var action = Get.CustomBuilderFor
                .MockAnarchyAction
                .WithCauseAnarchyType(anarchyType)
                .Build();

            var factory = Get.CustomBuilderFor
                .MockAnarchyActionFactory
                .WithAction(action)
                .ThatHasCanHandleResponse(canHandle);

            return factory.Build();
        }
    }
}
