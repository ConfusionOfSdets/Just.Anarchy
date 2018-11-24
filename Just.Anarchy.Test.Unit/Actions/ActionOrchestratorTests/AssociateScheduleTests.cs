using System;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.ActionOrchestratorTests
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
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, timer);

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
            var orchestrator = new ActionOrchestrator<ICauseAnarchy>(action, timer);
            orchestrator.Start();

            //Act
            Action sutCall = () => orchestrator.AssociateSchedule(schedule);

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
            var orchestrator = new ActionOrchestrator<ICauseAnarchy>(action, timer);

            //Act
            Action sutCall = () => orchestrator.AssociateSchedule(schedule);

            //Assert
            sutCall.Should().Throw<UnschedulableActionException>();
        }
    }
}
