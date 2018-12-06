using System.Net.Http;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Middleware
{
    [TestFixture]
    public class WhenAnarchyRouteIsRequested : BaseIntegrationTest
    {
        private HttpClient _client;
        private IActionOrchestrator _mockOrchestrator;

        public override void Given()
        {

            _mockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("testAction")
                .ThatCanHandleRequest()
                .Build();

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockOrchestrator));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            await _client.GetAsync("/anarchy/route");
        }

        [Then]
        public void TheActionHandleRequestWasNotTriggered()
        {
            _mockOrchestrator.DidNotReceive().HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>());
        }
    }
}