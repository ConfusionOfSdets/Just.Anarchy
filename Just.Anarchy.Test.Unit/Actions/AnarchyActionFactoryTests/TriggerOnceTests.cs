using System;
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
    public class TriggerOnceTests
    {
        [Test]
        [TestCase(null)]
        [TestCase(1)]
        public void TriggerOnceCallsActionExecuteAsync(int? durationSecs)
        {
            //Arrange
            var action = Get.CustomBuilderFor.MockAnarchyAction.ThatIsSchedulable().Build();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            var duration = durationSecs.HasValue ? TimeSpan.FromSeconds(durationSecs.Value) : (TimeSpan?)null;

            //Act
            sut.TriggerOnce(duration);

            //Assert
            action.Received(1).ExecuteAsync(duration, Arg.Any<CancellationToken>());
        }

        [Test]
        [TestCase(null)]
        [TestCase(1)]
        public void TriggerOnceSetsIsActive(int? durationSecs)
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(async ct => await Block.UntilCancelled(cts.Token))
                .Build();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            var duration = durationSecs.HasValue ? TimeSpan.FromSeconds(durationSecs.Value) : (TimeSpan?)null;

            //Act
            sut.TriggerOnce(duration);
            var runningIsActiveState = sut.IsActive;
            cts.Cancel();

            //Assert
            runningIsActiveState.Should().BeTrue();
        }

        [Test]
        [TestCase(null)]
        [TestCase(1)]
        public async Task TriggerOnceSetsIsActiveFalseAfterExecution(int? durationSecs)
        {
            //Arrange
            var triggered = false;
            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(ct => triggered = true)
                .Build();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            var duration = durationSecs.HasValue ? TimeSpan.FromSeconds(durationSecs.Value) : (TimeSpan?)null;

            //Act
            sut.TriggerOnce(duration);
            await Wait.Until(() => triggered, 1);
            await Wait.Until(() => sut.IsActive == false, 1);

            //Assert
            sut.IsActive.Should().BeFalse();
        }

        [Test]
        [TestCase(null)]
        [TestCase(1)]
        public async Task TriggerOnceSetsIsActiveFalseAfterActionExecutionThrows(int? durationSecs)
        {
            //Arrange
            var triggered = false;
            var action = Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsSchedulable()
                .ThatExecutesTask(ct =>
                {
                    triggered = true;
                    throw new Exception("this shouldn't affect things");
                })
                .Build();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            var duration = durationSecs.HasValue ? TimeSpan.FromSeconds(durationSecs.Value) : (TimeSpan?)null;

            //Act
            sut.TriggerOnce(duration);
            await Wait.Until(() => triggered, 1);
            await Wait.Until(() => sut.IsActive == false, 1);

            //Assert
            sut.IsActive.Should().BeFalse();
        }
    }
}
