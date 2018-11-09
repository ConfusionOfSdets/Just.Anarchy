using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core
{
    [TestFixture]
    public class SchedulerTests
    {
        [Test]
        public void ConstructorSetsSchedule()
        {
            //arrange
            var schedule = new Schedule();
            var action = Substitute.For<ICauseScheduledAnarchy, ICauseAnarchy>();
            //act
            var sut = new Scheduler(schedule, action, TestTimer.WithoutDelays());
            //assert
            sut.Schedule.Should().BeSameAs(schedule);
        }

        [Test]
        public async Task StartScheduleTriggersActionExecution()
        {

            //arrange
            var schedule = new Schedule().Repeat(1);
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithoutDelays());

            //act
            sut.StartSchedule();
            await Wait.Until(() => !sut.Running, 1);

            //assert
            await action
                .Received(1)
                .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task StartScheduleTriggersActionExecutionWithCorrectDurationPositive()
        {
            //arrange
            var oneMillisecond = TimeSpan.FromMilliseconds(1);
            var schedule = Get.CustomBuilderFor.Schedule
                .For(oneMillisecond)
                .Repeat(1);
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithoutDelays());

            //act
            sut.StartSchedule();
            await Wait.Until(() => !sut.Running, 1);

            //assert
            await action
                .Received(1)
                .ExecuteAsync(oneMillisecond, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task StartScheduleTriggersActionExecutionWithCorrectDurationNull()
        {
            //arrange;
            var schedule = Get.CustomBuilderFor.Schedule                .Repeat(1);
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithoutDelays());

            //act
            sut.StartSchedule();
            await Wait.Until(() => !sut.Running, 1);

            //assert
            await action
                .Received(1)
                .ExecuteAsync(null, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ScheduleStopsAfterRepeatCountMet()
        {
            var expRepetitions = 2;

            //arrange
            var schedule = new Schedule().Repeat(expRepetitions);
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithoutDelays());

            //act
            sut.StartSchedule();
            await Wait.Until(() => !sut.Running, 1);

            //assert
            await action
                .Received(expRepetitions)
                .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ScheduleTotalDurationOverridesRepeatCount()
        {
            var expExecutionCount = 2;
            var expTotalDuration = TimeSpan.FromSeconds(1.8);

            //arrange
            var schedule = new Schedule()
                .WithInterval(TimeSpan.FromSeconds(1))
                .FinishAfter(expTotalDuration)
                .Repeat(3);

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());

            //act
            sut.StartSchedule();
            var duration = await Wait.AndTimeActionAsync(() => !sut.Running, 1.9);

            //assert
            await action
                .Received(expExecutionCount)
                .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
            duration.Should().BeCloseTo(expTotalDuration, 300);
        }

        [Test]
        public async Task ScheduleTotalDurationOverridesRepeatWithoutEnd()
        {
            var expExecutionCount = 2;
            var expTotalDuration = TimeSpan.FromSeconds(0.5);

            //arrange
            var schedule = new Schedule()
                .WithInterval(TimeSpan.FromSeconds(0.3))
                .FinishAfter(expTotalDuration)
                .Repeat(0);

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());

            //act
            sut.StartSchedule();
            var duration = await Wait.AndTimeActionAsync(() => !sut.Running, 1);

            //assert
            await action
                .Received(expExecutionCount)
                .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
            duration.Should().BeCloseTo(expTotalDuration, 300);
        }

        [Test]
        public async Task ScheduleTotalDurationOverridesInterval()
        {
            var expTotalDuration = TimeSpan.FromSeconds(0.5);

            //arrange
            var schedule = new Schedule()
                .WithInterval(TimeSpan.FromSeconds(1))
                .FinishAfter(expTotalDuration)
                .Repeat(2);

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());

            //act
            sut.StartSchedule();
            var duration = await Wait.AndTimeActionAsync(() => !sut.Running, 1);

            //assert
            duration.Should().BeCloseTo(expTotalDuration, 300);
        }

        [Test]
        public async Task ScheduleTotalDurationOverridesInitialDelay()
        {
            var expTotalDuration = TimeSpan.FromSeconds(0.5);

            //arrange
            var schedule = new Schedule()
                .ToStartWithDelay(TimeSpan.FromSeconds(1))
                .FinishAfter(expTotalDuration)
                .Repeat(0);

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());

            //act
            sut.StartSchedule();
            var duration = await Wait.AndTimeActionAsync(() => !sut.Running, 1.2);

            //assert
            duration.Should().BeCloseTo(expTotalDuration, 500);
        }

        [Test]
        public async Task SchedulerDoesNotApplyIntervalAfterLastExecution()
        {
            //arrange
            var schedule = new Schedule()
                .WithInterval(TimeSpan.FromSeconds(1))
                .Repeat(2);

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());

            //act
            sut.StartSchedule();
            var duration = await Wait.AndTimeActionAsync(() => !sut.Running, 1.1);

            //assert
            await action
                .Received(2)
                .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
            duration.Should().BeCloseTo(TimeSpan.FromSeconds(1.1), 300);
        }

        [Test]
        public async Task SchedulerStopsScheduleInDelayState()
        {
            //arrange
            var schedule = new Schedule()
                .ToStartWithDelay(TimeSpan.FromSeconds(10))
                .WithoutEnd();

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());
            sut.StartSchedule();
            
            var monitorTask = Task.Run(async () => await Wait.AndTimeActionAsync(() => !sut.Running, 20));
            
            //act
            await Task.Delay(100);
            sut.StopSchedule();
            var duration = await monitorTask;

            //assert
            duration.Should().BeCloseTo(TimeSpan.FromSeconds(0.2), 200);
        }

        [Test]
        public async Task SchedulerStopsScheduleInIterationDelayState()
        {
            //arrange
            var schedule = new Schedule()
                .WithInterval(TimeSpan.FromSeconds(10))
                .WithoutEnd();

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithDelays());
            sut.StartSchedule();

            var monitorTask = Task.Run(async () => await Wait.AndTimeActionAsync(() => !sut.Running, 20));

            //act
            await Task.Delay(500);
            sut.StopSchedule();
            var duration = await monitorTask;

            //assert
            duration.Should().BeCloseTo(TimeSpan.FromSeconds(0.6), 200);
        }

        [Test]
        public async Task SchedulerStopsScheduleWhenExecuting()
        {
            //arrange
            var schedule = new Schedule()
                .WithoutEnd();
            var ctTriggered = false;

            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            action
                .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
                .Returns(async r =>
            {
                var ct = r.ArgAt<CancellationToken>(1);
                try
                {
                    await Task.Delay(20000, ct);
                }
                catch (TaskCanceledException)
                {
                    ctTriggered = true;
                }
            });

            var sut = new Scheduler(schedule, (ICauseScheduledAnarchy)action, TestTimer.WithoutDelays());
            sut.StartSchedule();

            var monitorTask = Task.Run(async () => await Wait.AndTimeActionAsync(() => !sut.Running, 20));

            //act
            await Task.Delay(500);
            sut.StopSchedule();
            var duration = await monitorTask;

            //assert
            duration.Should().BeCloseTo(TimeSpan.FromSeconds(0.6), 200);
            ctTriggered.Should().BeTrue();
        }
    }
}
