using System;
using System.Threading;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.ActionOrchestratorTests
{
    [TestFixture]
    public class StartTests
    {
        // Not Running, Schedulable
        [Test]
        public void Start_SchedulableAndScheduleIsSet_SetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            sut.AssociateSchedule(new Schedule());

            //Act
            sut.Start();

            //Assert
            sut.IsActive.Should().BeTrue();
        }

        [Test]
        public void Start_SchedulableAndScheduleIsSet_CallsStartSchedule()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            sut.AssociateSchedule(new Schedule());

            //Act
            sut.Start();

            //Assert
            scheduler.Received(1).StartSchedule();
        }

        [Test]
        public void Start_SchedulableAndScheduleNotSet_DoesNotSetIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            Assert.Catch(() => sut.Start());

            //Assert
            sut.IsActive.Should().BeFalse();
        }

        [Test]
        public void Start_SchedulableAndScheduleNotSet_DoesNotCallStartSchedule()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            Assert.Catch(() => sut.Start());

            //Assert
            scheduler.DidNotReceive().StartSchedule();
        }

        [Test]
        public void Start_SchedulableAndScheduleNotSet_Throws()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            var exception = Assert.Catch(() => sut.Start());

            //Assert
            exception.Should().BeOfType<ScheduleMissingException>();
        }

        // Not Running, NotSchedulable
        [Test]
        public void Start_NotSchedulable_DoesNotSetIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            Assert.Catch(() => sut.Start());

            //Assert
            sut.IsActive.Should().BeFalse();
        }

        [Test]
        public void Start_NotSchedulable_DoesNotCallStartSchedule()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            Assert.Catch(() => sut.Start());

            //Assert
            scheduler.DidNotReceive().StartSchedule();
        }

        [Test]
        public void Start_NotSchedulable_Throws()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            var exception = Assert.Catch(() => sut.Start());

            //Assert
            exception.Should().BeOfType<UnschedulableActionException>();
        }

        // Running, Schedulable
        [Test]
        public void Start_Running_DoesNotCallStartSchedule()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            sut.AssociateSchedule(new Schedule());
            sut.Start();
            scheduler.Running.Returns(true);
            scheduler.ClearReceivedCalls();

            //Act
            Assert.Catch(() => sut.Start());

            //Assert
            scheduler.DidNotReceive().StartSchedule();
        }

        [Test]
        public void Start_Running_Throws()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            sut.AssociateSchedule(new Schedule());
            sut.Start();
            scheduler.Running.Returns(true);

            //Act
            var exception = Assert.Catch(() => sut.Start());

            //Assert
            exception.Should().BeOfType<ScheduleRunningException>();
        }


        [Test]
        public void Start_ActionOrchestratorStopping_Throws()
        {
            //Arrange
            var ctsFromTest = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var initialExecution = true;

            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(async ctFromOrchestrator =>
                {
                    // the goal of this is to block the action execution on the first call,
                    // this will lead to an active task in _executionInstances that will need cancelling
                    if (initialExecution)
                    {
                        initialExecution = false;
                        await Block.UntilCancelled(ctsFromTest.Token);
                    }
                })
                .Build();

            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

#pragma warning disable 4014 // explicitly not awaiting here as we need to set separate tasks running that are blocked to trigger test state
            sut.TriggerOnce(TimeSpan.FromMinutes(1000)); // block the stop action to ensure we have a token that is cancelled but not replaced
            sut.Stop();
#pragma warning restore 4014

            //Act
            var exception = Assert.Catch(() => sut.Start());
            ctsFromTest.Cancel();

            //Assert
            exception.Should().BeOfType<ActionStoppingException>();
        }
    }
}
