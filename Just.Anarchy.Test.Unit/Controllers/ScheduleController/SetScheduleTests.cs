using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class SetScheduleTests : BaseControllerTests
    {
        const string anarchyType = "anarchyType";

        [Test]
        public void SetSchedule_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));
            var schedule = It.IsAny<Schedule>();
            
            //Act
            sut.SetSchedule(anarchyType, schedule);

            //Assert
            anarchyManager.Received(1).AssignScheduleToAnarchyActionFactory(anarchyType, schedule, false);
        }

        [Test]
        public void SetSchedule_NoExistingSchedule_ReturnsCreatedResult()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), false)
                .Returns(true);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            var schedule = It.IsAny<Schedule>();

            //Act
            var result = sut.SetSchedule(anarchyType, schedule);

            //Assert
            result.Should().BeOfType<CreatedResult>();
        }

        [Test]
        public void SetSchedule_NoExistingSchedule_ResultContainsSchedule()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            anarchyManager
                .AssignScheduleToAnarchyActionFactory(Arg.Any<string>(), Arg.Any<Schedule>(), false)
                .Returns(true);
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            var schedule = It.IsAny<Schedule>();

            //Act
            var result = sut.SetSchedule(anarchyType, schedule);

            //Assert
            ((CreatedResult)result).Value.Should().Be(schedule);
        }
    }
}
