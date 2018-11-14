using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Controllers;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Responses;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration.Controllers.ScheduleController
{
    [TestFixture]
    public class WhenAllSchedulesAreRetrieved : BaseIntegrationTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private string _responsePayload;
        private Schedule[] _schedules;

        public override void Given()
        {
            _schedules = new []
            {
                Get.CustomBuilderFor.Schedule.WithRandomValues(),
                Get.CustomBuilderFor.Schedule.WithRandomValues()
            };

            var factories = Get.CustomBuilderFor.MockAnarchyActionFactories
                .WithFactoriesWithActionsNamed("testAction1", "testAction2")
                .WithFactoriesWithSchedules(_schedules)
                .Build()
                .Select(f => f.Build())
                .ToList();

            var webApplicationFactory = new CustomWebApplicationFactory(builder =>
            {
                factories.ForEach(f => builder.AddSingleton(f));
            });

            _client = webApplicationFactory.CreateClient();
        }

        public override async Task WhenAsync()
        {
            _response = await _client.GetAsync(Routes.Schedule.GetAll);
            _responsePayload = _response.Content.ReadAsStringAsync().Result;
        }

        [Then]
        public void ThenTheResponseHasTheCorrectStatus()
        {
            _response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Then]
        public void ThenTheResponseHasTheCorrectListOfSchedules()
        {
            var expectedResponse = new EnumerableResultResponse<NamedScheduleDto>(new []
            {
                new NamedScheduleDto("CpuAnarchy", null),
                new NamedScheduleDto("MemoryAnarchy", null),
                new NamedScheduleDto("testAction1", _schedules[0]),
                new NamedScheduleDto("testAction2", _schedules[1])
            });

            JsonConvert.DeserializeObject<EnumerableResultResponse<NamedScheduleDto>>(_responsePayload)
                .Should()
                .BeEquivalentTo(expectedResponse);
        }
    }
}