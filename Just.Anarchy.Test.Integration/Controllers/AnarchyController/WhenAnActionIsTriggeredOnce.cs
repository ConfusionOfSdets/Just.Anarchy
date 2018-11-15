using System;
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

namespace Just.Anarchy.Test.Integration.Controllers.AnarchyController
{
    [TestFixture]
    public class WhenAnActionIsTriggeredOnce : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private IAnarchyActionFactory _mockFactory;

        public override void Given()
        {
            _mockFactory = Get.MotherFor.MockAnarchyActionFactory
                .FactoryWithoutScheduleNamed("testAction")
                .WithIsActive(false)
                .Build();

            var factory = new CustomWebApplicationFactory(builder => builder.AddSingleton(_mockFactory));

            _client = factory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.PostAsync(
                    Routes.Anarchy.Trigger.Replace("{anarchyType}", "testAction"),
                new StringContent(JsonConvert.SerializeObject(new Schedule()), Encoding.UTF8, "application/json"));
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Then]
        public void TheActionFactoryHasAScheduleSet()
        {
            _mockFactory.Received(1).TriggerOnce(Arg.Any<TimeSpan?>());
        }
    }
}