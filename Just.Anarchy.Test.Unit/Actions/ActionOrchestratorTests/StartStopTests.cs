using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.ActionOrchestratorTests
{
    [TestFixture]
    public class StartStopTests
    {
        [Test]
        public void StartSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            sut.Start();

            //Assert
            sut.IsActive.Should().BeTrue();
        }

        [Test]
        public void StartCallsStartScheduleIfScheduleSet()
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
        public void StartDoesNotCallStartScheduleIfScheduleIsNotSet()
        {
            //Arrange
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var scheduler = Substitute.For<IScheduler>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory
                .CreatesSpecifiedScheduler(scheduler)
                .Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            sut.Start();

            //Assert
            scheduler.DidNotReceive().StartSchedule();
        }

        [Test]
        public void StartErrorsWhenActionOrchestratorIsStopping()
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

        [Test]
        public async Task StopSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);

            //Act
            await sut.Stop();

            //Assert
            sut.IsActive.Should().BeFalse();
        }


        [Test]
        public async Task StopCallsStopSchedule()
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

            //Act
            await sut.Stop();

            //Assert
            scheduler.Received(1).StopSchedule();
        }

        [Test]
        public async Task StopKillsUnscheduledExecutions()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            CancellationToken linkedCancellationToken;

            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(async ctFromOrchestrator =>
                {
                    linkedCancellationToken = 
                        CancellationTokenSource.CreateLinkedTokenSource(ctFromOrchestrator, ctFromTest).Token;
                    await Block.UntilCancelled(linkedCancellationToken);
                })
                .Build();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            sut.TriggerOnce(null);

            //Act
#pragma warning disable CS4014 // Intentionally not awaiting Stop as we want to check the task triggered
            sut.Stop();
#pragma warning restore CS4014 // We instead wait until our test state is triggered before asserting
            await Wait.Until(() => linkedCancellationToken.IsCancellationRequested, 1);
            var stopCancelledTheTask = !ctFromTest.IsCancellationRequested;
            cts.Cancel();

            //Assert
            Assert.That(stopCancelledTheTask);
        }
    }
}
