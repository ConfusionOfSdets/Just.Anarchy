using System.Collections.Generic;
using FluentAssertions;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Responses;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class GetAllSchedulesTests
    {
        [Test]
        public void GetAllSchedules_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var schedules = new List<NamedScheduleDto>();

            anarchyManager.GetAllSchedulesFromOrchestrators().Returns(schedules);
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager);
            
            //Act
            var result = sut.GetAllSchedules();

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            var expected = new EnumerableResultResponse<NamedScheduleDto>(schedules);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(expected);
        }
    }
}
