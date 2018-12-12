using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Middleware.WhenNonAnarchyRouteIsRequested
{
    [TestFixture]
    public class WithMultipleActionOrchestrators : BaseIntegrationTest
    {
        private HttpClient _client;
        private IActionOrchestrator _passiveMockOrchestrator;
        private IActionOrchestrator _alterResponseMockOrchestrator;
        private DateTime _passiveCalledAt;
        private DateTime _alterResponseCalledAt;
        private HttpResponseMessage _response;
        
        public override void Given()
        {
            _passiveMockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("goingtodonothing")
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction.WithCauseAnarchyType(CauseAnarchyType.Passive).Build())
                .ThatCanHandleRequestWith(async (context, next) =>
                {
                    _passiveCalledAt = DateTime.Now;
                    await Task.Delay(100);
                })
                .Build();

            _alterResponseMockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("teapot")
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction.WithCauseAnarchyType(CauseAnarchyType.AlterResponse).Build())
                .ThatCanHandleRequestWith(async (context, next) =>
                {
                    _alterResponseCalledAt = DateTime.Now;
                    context.Response.StatusCode = StatusCodes.Status418ImATeapot;
                    await context.Response.WriteAsync("Im A Little Teapot Short and Stout...");
                })
                .Build();

            var factory = new CustomWebApplicationFactory(builder =>
            {
                builder.AddSingleton(_passiveMockOrchestrator);
                builder.AddSingleton(_alterResponseMockOrchestrator);
            });
            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.GetAsync("/nonanarchy/route");
        }

        [Then]
        public void ThePassiveActionHandleRequestWasTriggered()
        {
            _passiveMockOrchestrator.Received(1).HandleRequest(Arg.Is<HttpContext>(h => h.Request.Path == "/nonanarchy/route"), Arg.Any<RequestDelegate>());
        }

        [Then]
        public void TheAlterResponseActionHandleRequestWasTriggered()
        {
            _alterResponseMockOrchestrator.Received(1).HandleRequest(Arg.Is<HttpContext>(h => h.Request.Path == "/nonanarchy/route"), Arg.Any<RequestDelegate>());
        }

        [Then]
        public void TheAlterResponseActionWasCalledLast()
        {
            _alterResponseCalledAt.Should().BeAfter(_passiveCalledAt);
        }

        [Then]
        public void TheResponseHasTheExpectedStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status418ImATeapot);
        }

        [Then]
        public async Task TheResponseHasTheExpectedContent()
        {
            (await _response.Content.ReadAsStringAsync()).Should().Be("Im A Little Teapot Short and Stout...");
        }
    }
}