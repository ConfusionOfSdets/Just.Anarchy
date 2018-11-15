using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
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
            var sut = new AnarchyActionFactory(action, timer);

            //Act
            sut.Start();

            //Assert
            sut.IsActive.Should().BeTrue();
        }

        [Test]
        public void StopSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

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
            var sut = new AnarchyActionFactory(action, timer);
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
