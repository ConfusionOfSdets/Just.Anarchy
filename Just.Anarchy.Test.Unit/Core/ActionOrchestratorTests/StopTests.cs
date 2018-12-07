using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.ActionOrchestratorTests
{
    [TestFixture]
    public class StopTests
    {
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
