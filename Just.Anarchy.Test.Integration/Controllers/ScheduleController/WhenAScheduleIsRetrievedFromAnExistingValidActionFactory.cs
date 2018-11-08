using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Test.Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.ScheduleController
{
    [TestFixture]
    public class WhenAScheduleIsRetrievedFromAnExistingValidActionFactory : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private string _responsePayload;
        private Schedule _schedule;
        private bool _scheduleRetrieved = false;
        private IAnarchyActionFactory _mockFactory;

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

            var mockAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            mockAction.Name.Returns("testAction");

            _mockFactory = Substitute.For<IAnarchyActionFactory>();
            _mockFactory.AnarchyAction.Returns(mockAction);
            _mockFactory.ExecutionSchedule.Returns(_schedule).AndDoes(_ => _scheduleRetrieved = true);
            _mockFactory.IsActive.Returns(false);

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockFactory));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.GetAsync(
                Routes.GetSetSchedule.Replace("{anarchyType}", "testAction"));
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
        public void ThenTheScheduleWasRetrievedFromTheActionFactory()
        {
            _scheduleRetrieved.Should().BeTrue();
        }
    }
}