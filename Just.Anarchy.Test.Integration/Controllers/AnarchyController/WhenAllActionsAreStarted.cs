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
    public class WhenAllActionsAreStarted : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IActionOrchestrator _mockUnscheduledOrchestrator;
        private IActionOrchestrator _mockScheduledOrchestrator;

        public override void Given()
        {
            _mockUnscheduledOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithUnschedulableAction
                .Build();

            _mockScheduledOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("test")
                .Build();

            var factory = new CustomWebApplicationFactory(builder =>
            {
                builder.AddSingleton(_mockUnscheduledOrchestrator);
                builder.AddSingleton(_mockScheduledOrchestrator);
            });

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        { 
            _response = await _client.PutAsync(Routes.Schedule.StartAll, null);
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Then]
        public void AllScheduledActionOrchestratorsHaveBeenAskedToStart()
        {
            _mockUnscheduledOrchestrator.DidNotReceive().Start();
            _mockScheduledOrchestrator.Received(1).Start();
        }
    }
}