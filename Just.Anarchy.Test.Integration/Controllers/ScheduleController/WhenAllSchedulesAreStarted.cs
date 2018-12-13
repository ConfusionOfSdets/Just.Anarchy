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
    public class WhenAllSchedulesAreStarted : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IActionOrchestrator _mockOrchestratorWithSchedule;
        private IActionOrchestrator _mockOrchestratorWithoutSchedule;
        private IActionOrchestrator _mockOrchestratorUnschedulable;

        public override void Given()
        {
            _mockOrchestratorWithSchedule = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAction1")
                .WithIsActive(false)
                .Build();

            _mockOrchestratorWithoutSchedule = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAction2")
                .WithIsActive(false)
                .Build();

            _mockOrchestratorUnschedulable = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithUnschedulableActionNamed("testAction3")
                .WithIsActive(false)
                .Build();

            var factory = new CustomWebApplicationFactory(builder =>
            {
                builder.AddSingleton(_mockOrchestratorWithSchedule);
                builder.AddSingleton(_mockOrchestratorWithoutSchedule);
                builder.AddSingleton(_mockOrchestratorUnschedulable);
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

        [Then] public void AllSchedulableActionOrchestratorsHaveReceivedACallToStart()
        {
            _mockOrchestratorWithSchedule.Received(1).Start();
            _mockOrchestratorWithoutSchedule.Received(1).Start();
            _mockOrchestratorUnschedulable.DidNotReceive().Start();
        }
    }
}