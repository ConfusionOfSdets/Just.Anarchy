using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class StartAllSchedulesTests : BaseControllerTests
    {
        [Test]
        public void StartSchedule_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.ScheduleController>>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager, logger);
            
            //Act
            sut.StartAllSchedules();

            //Assert
            anarchyManager.Received(1).StartAllSchedules();
        }

        [Test]
        public void StartSchedule_ReturnsOkResult()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.ScheduleController>>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager, logger);

            //Act
            var result = sut.StartAllSchedules();

            //Assert
            result.Should().BeOfType<AcceptedResult>();
        }
    }
}
