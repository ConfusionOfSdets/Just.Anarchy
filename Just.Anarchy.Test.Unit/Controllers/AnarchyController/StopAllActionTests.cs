using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.AnarchyController
{
    [TestFixture]
    public class StopAllActionTests : BaseControllerTests
    {
        [Test]
        public void StopAllActions_ReturnsAcceptedResult()
        {
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.AnarchyController>>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager, logger);

            //Act
            var result = sut.StopAllActions();

            //Assert
            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public void StopAction_CallsAnarchyManager()
        {
            //arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.AnarchyController>>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager, logger);

            //Act
            sut.StopAllActions();

            //Assert
            anarchyManager.Received(1).StopAllActions();
        }
    }
}
