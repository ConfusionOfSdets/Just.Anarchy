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
    public class StartScheduleTests : BaseControllerTests
    {
        const string anarchyType = "anarchyType";

        [Test]
        public void StartSchedule_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));
            
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
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.ScheduleController(anarchyManager));

            //Act
            var result = sut.StartSchedule(anarchyType);

            //Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
