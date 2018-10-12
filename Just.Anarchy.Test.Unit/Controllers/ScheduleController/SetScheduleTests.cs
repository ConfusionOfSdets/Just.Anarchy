using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Just.Anarchy.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.ScheduleController
{
    [TestFixture]
    public class SetScheduleTests
    {
        const string anarchyType = "anarchyType";

        [Test]
        public void SetSchedule_CallsAnarchyManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = new Anarchy.Controllers.ScheduleController(anarchyManager);
            var schedule = new Schedule();
            
            //Act
            sut.SetSchedule(anarchyType, schedule);

            //Assert
            anarchyManager.Received(1).AssignScheduleToAnarchyActionFactory(anarchyType, schedule);
        }
    }
}
