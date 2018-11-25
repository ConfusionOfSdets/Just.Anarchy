using System;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockSchedulerFactoryBuilder
    {
        private Func<Schedule, ICauseScheduledAnarchy, IHandleTime, IScheduler> _createSchedulerAction = (schedule, action, timer) => new Scheduler(schedule, action, timer);

        public MockSchedulerFactoryBuilder()
        {
            
        }

        public MockSchedulerFactoryBuilder CreatesSpecifiedScheduler(IScheduler scheduler)
        {
            _createSchedulerAction = (schedule, action, timer) => scheduler;
            return this;
        }

        public ISchedulerFactory Build()
        {
            var schedulerFactory = Substitute.For<ISchedulerFactory>();
            schedulerFactory.CreateSchedulerForAction(Arg.Any<Schedule>(), Arg.Any<ICauseScheduledAnarchy>()).Returns(a => _createSchedulerAction(a.ArgAt<Schedule>(0), a.ArgAt<ICauseScheduledAnarchy>(1), Substitute.For<IHandleTime>()));
            return schedulerFactory;
        }
    }
}

