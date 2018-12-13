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

namespace Just.Anarchy.Test.Integration.Controllers.AnarchyController
{
    [TestFixture]
    public class WhenAllActionsAreStopped : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IActionOrchestrator _mockOrchestrator1;
        private IActionOrchestrator _mockOrchestrator2;

        public override void Given()
        {
            _mockOrchestrator1 = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("testAction1")
                .WithIsActive(false)
                .Build();

            _mockOrchestrator2 = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("testAction2")
                .WithIsActive(false)
                .Build();

            var factory = new CustomWebApplicationFactory(builder =>
            {
                builder.AddSingleton(_mockOrchestrator1);
                builder.AddSingleton(_mockOrchestrator2);
            });

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        { 
            _response = await _client.PutAsync(Routes.Anarchy.StopAllActions, null);
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Then]
        public void AllActionOrchestratorsHaveBeenAskedToStop()
        {
            _mockOrchestrator1.Received(1).Stop();
            _mockOrchestrator2.Received(1).Stop();
        }
    }
}