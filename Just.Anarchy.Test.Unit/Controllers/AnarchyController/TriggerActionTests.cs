using System;
using System.Collections.Generic;
using FluentAssertions;
using Just.Anarchy.Core.Dtos;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Responses;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);
            
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
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);

            //Act
            sut.TriggerAction("aFakeAnarchyType", null);

            //Assert
            anarchyManager.Received(1).TriggerAction("aFakeAnarchyType", TimeSpan.FromSeconds(10));
        }

        [Test]
        public void TriggerAction_CallsAnarchyManager_DurationOverridden()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);

            //Act
            sut.TriggerAction("aFakeAnarchyType", 1);

            //Assert
            anarchyManager.Received(1).TriggerAction("aFakeAnarchyType", TimeSpan.FromSeconds(1));
        }
    }
}
