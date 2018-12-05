using System;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.ActionOrchestratorTests
{
    [TestFixture]
    public class AssociateScheduleTests
    {
        [Test]
        public void AssociateScheduleSetsScheduleCorrectly()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var schedule = new Schedule();

            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

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
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var orchestrator = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
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
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var orchestrator = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            Action sutCall = () => orchestrator.AssociateSchedule(schedule);

            //Assert
            sutCall.Should().Throw<UnschedulableActionException>();
        }
    }
}
