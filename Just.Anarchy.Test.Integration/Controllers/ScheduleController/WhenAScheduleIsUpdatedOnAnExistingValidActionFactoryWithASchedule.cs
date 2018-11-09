using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.ScheduleController
{
    [TestFixture]
    public class WhenAScheduleIsUpdatedOnAnExistingValidActionFactoryWithASchedule : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IAnarchyActionFactory _mockFactory;

        public override void Given()
        {
            var mockAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            mockAction.Name.Returns("testAction");

            _mockFactory = Substitute.For<IAnarchyActionFactory>();
            _mockFactory.AnarchyAction.Returns(mockAction);
            _mockFactory.ExecutionSchedule.Returns(new Schedule());
            _mockFactory.IsActive.Returns(false);

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockFactory));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.PutAsync(
                Routes.Schedule.GetSetUpdate.Replace("{anarchyType}", "testAction"),
                new StringContent(JsonConvert.SerializeObject(new Schedule()), Encoding.UTF8, "application/json"));
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Then]
        public void TheActionFactoryHasAScheduleSet()
        {
            _mockFactory.Received(1).AssociateSchedule(Arg.Any<Schedule>());
        }
    }
}