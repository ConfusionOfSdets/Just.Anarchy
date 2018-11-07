using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class UpdateScheduleTests : BaseControllerTests
    {
        const string anarchyType = "anarchyType";

        [Test]
        public void UpdateSchedule_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), Arg.Any<bool>())
                .Returns(true);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            var schedule = new Schedule();
            
            //Act
            sut.UpdateSchedule(anarchyType, schedule);

            //Assert
            anarchyManager.Received(1).AssignScheduleToAnarchyActionFactory(anarchyType, schedule, Arg.Any<bool>());
        }

        [Test]
        public void UpdateSchedule_NoExistingSchedule_ReturnsCreatedResult()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), true)
                .Returns(true);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            var schedule = new Schedule();

            //Act
            var result = sut.UpdateSchedule(anarchyType, schedule);

            //Assert
            result.Should().BeOfType<CreatedResult>();
        }

        [Test]
        public void UpdateSchedule_NoExistingSchedule_ResultContainsSchedule()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), true)
                .Returns(true);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            var schedule = new Schedule();

            //Act
            var result = sut.UpdateSchedule(anarchyType, schedule);

            //Assert
            ((CreatedResult)result).Value.Should().Be(schedule);
        }

        [Test]
        public void UpdateSchedule_ExistingSchedule_ReturnsOkResult()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), true)
                .Returns(false);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));
            
            var schedule = new Schedule();

            //Act
            var result = sut.UpdateSchedule(anarchyType, schedule);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void UpdateSchedule_ExistingSchedule_ResultContainsSchedule()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), true)
                .Returns(false);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));
            var schedule = new Schedule();

            //Act
            var result = sut.UpdateSchedule(anarchyType, schedule);

            //Assert
            ((OkObjectResult)result).Value.Should().Be(schedule);
        }
    }
}
