using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Requests;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.AnarchyController
{
    [TestFixture]
    public class WhenATargetPatternIsSet : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private EnableOnRequestHandlingRequest _payload;
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
            _payload = new EnableOnRequestHandlingRequest
            {
                TargetPattern = ".*"
            };

            _response = await _client.PostAsync(
                    Routes.Anarchy.SetOrCancelOnRequestHandling.Replace("{anarchyType}", "testAction"),
                new StringContent(JsonConvert.SerializeObject(_payload), Encoding.UTF8, "application/json"));
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Then]
        public void TheActionOrchestratorHasBeenAskedToSetTheTarget()
        {
            _mockOrchestrator.Received(1).ForTargetPattern(_payload.TargetPattern);
        }
    }
}