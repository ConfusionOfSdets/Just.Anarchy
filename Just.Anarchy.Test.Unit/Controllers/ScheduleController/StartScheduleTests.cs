using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class StartScheduleTests : BaseControllerTests
    {
        const string anarchyType = "anarchyType";

        [Test]
        public void StartSchedule_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.ScheduleController>>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager, logger);
            
            //Act
            sut.StartSchedule(anarchyType);

            //Assert
            anarchyManager.Received(1).StartSchedule(anarchyType);
        }

        [Test]
        public void StartSchedule_ReturnsOkResult()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.ScheduleController>>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager, logger);

            //Act
            var result = sut.StartSchedule(anarchyType);

            //Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
