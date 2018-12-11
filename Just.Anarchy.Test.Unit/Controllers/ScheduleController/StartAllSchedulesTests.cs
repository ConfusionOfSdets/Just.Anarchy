using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));
            
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
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            //Act
            var result = sut.StartAllSchedules();

            //Assert
            result.Should().BeOfType<AcceptedResult>();
        }
    }
}
