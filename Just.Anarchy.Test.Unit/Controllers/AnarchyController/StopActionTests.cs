using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);

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
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);

            //Act
            sut.StopAction(actionType);

            //Assert
            anarchyManager.Received(1).StopAction(actionType);
        }
    }
}
