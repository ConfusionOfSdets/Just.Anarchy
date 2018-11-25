using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core
{
    [TestFixture]
    public class SchedulerFactoryTests
    {
        [Test]
        public void CreateSchedulerForAction_ReturnsScheduler()
        {
            //arrange
            var timer = Substitute.For<IHandleTime>();
            var schedule = new Schedule();
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var sut = new SchedulerFactory(timer);
            //act
            var result = sut.CreateSchedulerForAction(schedule, action);
            //assert
            result.Should().BeOfType<Scheduler>();
        }

        [Test]
        public void CreateSchedulerForAction_SetsSchedule()
        {
            //arrange
            var timer = Substitute.For<IHandleTime>();
            var schedule = new Schedule();
            var action = Substitute.For<ICauseScheduledAnarchy>();
            var sut = new SchedulerFactory(timer);
            //act
            var result = sut.CreateSchedulerForAction(schedule, action);
            //assert
            result.Schedule.Should().BeSameAs(schedule);
        }

        [Test]
        public async Task CreateSchedulerForAction_PassesActionCorrectly()
        {
            //arrange
            var timer = Substitute.For<IHandleTime>();
            var schedule = new Schedule
            {
                Delay = TimeSpan.Zero,
                IterationDuration = TimeSpan.FromSeconds(1),
                RepeatCount = 1
            };

            var action = Substitute.For<ICauseScheduledAnarchy>();
            var sut = new SchedulerFactory(timer);

            //act
            var scheduler = sut.CreateSchedulerForAction(schedule, action);
            scheduler.StartSchedule();

            // as we can't await StartSchedule (intentionally) we have to resort to a short delay to ensure the action is called consistently
            await Task.Delay(100); 

            //assert
            await action.Received(1).ExecuteAsync(schedule.IterationDuration, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task CreateSchedulerForAction_PassesTimerCorrectly()
        {
            //arrange
            var timer = Substitute.For<IHandleTime>();
            var schedule = new Schedule
            {
                Delay = TimeSpan.Zero,
                IterationDuration = TimeSpan.FromSeconds(1),
                RepeatCount = 1
            };

            var action = Substitute.For<ICauseScheduledAnarchy>();
            var sut = new SchedulerFactory(timer);

            //act
            var scheduler = sut.CreateSchedulerForAction(schedule, action);
            scheduler.StartSchedule();

            // as we can't await StartSchedule (intentionally) we have to resort to a short delay to ensure the timer is called consistently
            await Task.Delay(100);

            //assert
            await timer.Received(1).DelayInitial(schedule, Arg.Any<CancellationToken>());
        }
    }
}
