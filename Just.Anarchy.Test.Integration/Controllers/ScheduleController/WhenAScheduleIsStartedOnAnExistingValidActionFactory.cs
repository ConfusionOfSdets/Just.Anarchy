using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.ScheduleController
{
    [TestFixture]
    public class WhenAScheduleIsStartedOnAnExistingValidActionFactory : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IActionOrchestrator _mockOrchestrator;

        public override void Given()
        {
            _mockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAction")
                .WithIsActive(false)
                .Build();

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockOrchestrator));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.PutAsync(Routes.Schedule.Start.Replace("{anarchyType}", "testAction"), null);
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Then]
        public void TheActionOrchestratorHasRecievedACallToStart()
        {
            _mockOrchestrator.Received(1).Start();
        }
    }
}