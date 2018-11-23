using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.AnarchyActionFactoryTests
{
    [TestFixture]
    public class StartStopTests
    {
        [Test]
        public void StartSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory<ICauseAnarchy>(action, timer);

            //Act
            sut.Start();

            //Assert
            sut.IsActive.Should().BeTrue();
        }

        [Test]
        public void StartErrorsWhenActionFactoryIsStopping()
        {
            //Arrange
            var ctsFromTest = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var initialExecution = true;

            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(async ctFromFactory =>
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

            var timer = Substitute.For<IHandleTime>();

            var sut = new AnarchyActionFactory<ICauseAnarchy>(action, timer);

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
        public void StopSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory<ICauseAnarchy>(action, timer);

            //Act
            sut.Stop();

            //Assert
            sut.IsActive.Should().BeFalse();
        }

        [Test]
        public async Task StopKillsUnscheduledExecutions()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var tokenFromTest = cts.Token;
            CancellationToken linkedCancellationToken;

            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(async tokenFromActionFactory =>
                {
                    linkedCancellationToken = 
                        CancellationTokenSource.CreateLinkedTokenSource(tokenFromActionFactory, tokenFromTest).Token;
                    await Block.UntilCancelled(linkedCancellationToken);
                })
                .Build();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory<ICauseAnarchy>(action, timer);
            sut.TriggerOnce(null);

            //Act
            sut.Stop();
            await Wait.Until(() => linkedCancellationToken.IsCancellationRequested, 1);
            var stopCancelledTheTask = !tokenFromTest.IsCancellationRequested;
            cts.Cancel();

            //Assert
            Assert.That(stopCancelledTheTask);
        }
    }
}
