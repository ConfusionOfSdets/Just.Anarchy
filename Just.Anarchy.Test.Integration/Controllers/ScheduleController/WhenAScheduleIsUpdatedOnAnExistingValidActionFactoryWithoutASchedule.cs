using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.ScheduleController
{
    [TestFixture]
    public class WhenAScheduleIsUpdatedOnAnExistingValidActionFactoryWithoutASchedule : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IAnarchyActionFactory _mockFactory;
        private Schedule _schedule;

        public override void Given()
        {
            var mockAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            mockAction.Name.Returns("testAction");

            _mockFactory = Get.MotherFor.MockAnarchyActionFactory
                .FactoryWithoutScheduleNamed("testAction")
                .WithIsActive(false)
                .Build();

            _mockFactory.AssociateSchedule(Arg.Any<Schedule>()).Returns(true);

            _schedule = new Schedule
            {
                Delay = TimeSpan.FromDays(1),
                Interval = TimeSpan.FromMilliseconds(2),
                IterationDuration = TimeSpan.FromMinutes(0),
                RepeatCount = 4,
                TotalDuration = TimeSpan.FromMinutes(1)
            };

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockFactory));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.PutAsync(
                Routes.Schedule.GetSetUpdate.Replace("{anarchyType}", "testAction"),
                new StringContent(JsonConvert.SerializeObject(_schedule), Encoding.UTF8, "application/json"));
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Then]
        public void AndTheActionFactoryHasAScheduleSet()
        {
            _mockFactory.Received(1).AssociateSchedule(Arg.Any<Schedule>());
        }
    }
}