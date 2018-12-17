using System;
using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.AnarchyController
{
    [TestFixture]
    public class TriggerActionTests
    {
        [Test]
        public void TriggerAction_ReturnsAcceptedResult()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.AnarchyController>>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager, logger);
            
            //Act
            var result = sut.TriggerAction("aFakeAnarchyType", null);

            //Assert
            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public void TriggerAction_CallsAnarchyManager_DurationNull()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.AnarchyController>>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager, logger);

            //Act
            sut.TriggerAction("aFakeAnarchyType", null);

            //Assert
            anarchyManager.Received(1).TriggerAction("aFakeAnarchyType", null);
        }

        [Test]
        public void TriggerAction_CallsAnarchyManager_DurationSpecified()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var logger = Substitute.For<ILogger<Anarchy.Controllers.AnarchyController>>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager, logger);

            //Act
            sut.TriggerAction("aFakeAnarchyType", 1);

            //Assert
            anarchyManager.Received(1).TriggerAction("aFakeAnarchyType", TimeSpan.FromSeconds(1));
        }
    }
}
