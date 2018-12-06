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
    public class WithAlterResponseActionOrchestrator : BaseIntegrationTest
    {
        private HttpClient _client;
        private IActionOrchestrator _mockOrchestrator;
        private HttpResponseMessage _response;

        public override void Given()
        {
            _mockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("teapot")
                .WithActionCauseAnarchyType(CauseAnarchyType.AlterResponse)
                .ThatCanHandleRequestWith(async (context, next) =>
                {
                    context.Response.StatusCode = StatusCodes.Status418ImATeapot;
                    await context.Response.WriteAsync("Im A Little Teapot Short and Stout...");
                })
                .Build();

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockOrchestrator));
            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.GetAsync("/nonanarchy/route");
        }

        [Then]
        public void TheActionHandleRequestWasTriggered()
        {
            _mockOrchestrator.Received(1).HandleRequest(Arg.Is<HttpContext>(h => h.Request.Path == "/nonanarchy/route"), Arg.Any<RequestDelegate>());
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