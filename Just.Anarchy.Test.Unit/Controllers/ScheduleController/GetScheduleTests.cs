using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class GetScheduleTests
    {
        const string AnarchyType = "anarchyType";

        [Test]
        public void GetSchedule_CallsAnarchyManager_ActionFound()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var schedule = It.IsAny<Schedule>();
            anarchyManager.GetScheduleFromActionOrchestrator(Arg.Any<string>()).Returns(schedule);
            var logger = Substitute.For<ILogger<Anarchy.Controllers.ScheduleController>>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager, logger);
            
            //Act
            var result = sut.GetSchedule(AnarchyType);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(schedule);
        }

        [Test]
        public void GetSchedule_CallsAnarchyManager_ScheduleEmpty()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager.GetScheduleFromActionOrchestrator(Arg.Any<string>()).Returns((Schedule)null);
            var logger = Substitute.For<ILogger<Anarchy.Controllers.ScheduleController>>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager, logger);

            //Act
            var result = sut.GetSchedule(AnarchyType);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
