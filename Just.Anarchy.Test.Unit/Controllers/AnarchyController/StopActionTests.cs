using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.AnarchyController
{
    [TestFixture]
    public class StopActionTests : BaseControllerTests
    {
        const string actionType = "aFakeAnarchyType";

        [Test]
        public void StopAction_ReturnsAcceptedResult()
        {
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.AnarchyController>>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager, logger);

            //Act
            var result = sut.StopAction(actionType);

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
            sut.StopAction(actionType);

            //Assert
            anarchyManager.Received(1).StopAction(actionType);
        }
    }
}
