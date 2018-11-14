using System;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.AnarchyActionFactoryTests
{
    [TestFixture]
    public class AssociateScheduleTests
    {
        [Test]
        public void AssociateScheduleSetsScheduleCorrectly()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var schedule = new Schedule();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            //Act
            sut.AssociateSchedule(schedule);

            //Assert
            sut.ExecutionSchedule.Should().BeSameAs(schedule);
        }

        [Test]
        public void AssociateScheduleCannotSetScheduleWhenActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var schedule = new Schedule();
            var timer = Substitute.For<IHandleTime>();
            var factory = new AnarchyActionFactory(action, timer);
            factory.Start();

            //Act
            Action sutCall = () => factory.AssociateSchedule(schedule);

            //Assert
            sutCall.Should().Throw<ScheduleRunningException>();
        }

        [Test]
        public void AssociateScheduleCannotSetScheduleForUnscheduledAction()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedule = new Schedule();
            var timer = Substitute.For<IHandleTime>();
            var factory = new AnarchyActionFactory(action, timer);

            //Act
            Action sutCall = () => factory.AssociateSchedule(schedule);

            //Assert
            sutCall.Should().Throw<UnschedulableActionException>();
        }
    }
}
