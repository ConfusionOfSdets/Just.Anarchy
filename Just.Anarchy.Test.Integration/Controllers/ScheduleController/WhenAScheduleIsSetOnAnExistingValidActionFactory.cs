using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.ScheduleController
{
    [TestFixture]
    public class WhenAScheduleIsSetOnAnExistingValidActionOrchestrator : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IActionOrchestrator _mockOrchestrator;

        public override void Given()
        {
            _mockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithoutScheduleNamed("testAction")
                .WithIsActive(false)
                .Build();

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockOrchestrator));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.PostAsync(
                    Routes.Schedule.GetSetUpdate.Replace("{anarchyType}", "testAction"),
                new StringContent(JsonConvert.SerializeObject(new Schedule()), Encoding.UTF8, "application/json"));
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Then]
        public void TheActionOrchestratorHasAScheduleSet()
        {
            _mockOrchestrator.Received(1).AssociateSchedule(Arg.Any<Schedule>());
        }
    }
}