using System;
using System.Net.Http;
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
    public class WhenAScheduleIsRetrievedFromAnExistingValidActionOrchestrator : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private string _responsePayload;
        private Schedule _schedule;
        private bool _scheduleRetrieved = false;
        private IActionOrchestrator _mockOrchestrator;

        public override void Given()
        {
            _schedule = new Schedule
            {
                Delay = TimeSpan.FromSeconds(1),
                Interval = TimeSpan.FromSeconds(2),
                IterationDuration = TimeSpan.FromMinutes(1),
                RepeatCount = 3,
                TotalDuration = TimeSpan.FromDays(1)
            };

            _mockOrchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAction")
                .WithIsActive(false)
                .Build();

            _mockOrchestrator.ExecutionSchedule.Returns(_schedule).AndDoes(_ => _scheduleRetrieved = true);

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockOrchestrator));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.GetAsync(
                Routes.Schedule.GetSetUpdate.Replace("{anarchyType}", "testAction"));
            _responsePayload = _response.Content.ReadAsStringAsync().Result;
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Then]
        public void ThenTheResponseHasTheCorrectSchedule()
        {
            
            JsonConvert.DeserializeObject<Schedule>(_responsePayload).Should().BeEquivalentTo(_schedule);
        }

        [Then]
        public void ThenTheScheduleWasRetrievedFromTheActionOrchestrator()
        {
            _scheduleRetrieved.Should().BeTrue();
        }
    }
}