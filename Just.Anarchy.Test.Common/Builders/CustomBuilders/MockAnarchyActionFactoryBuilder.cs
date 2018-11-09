using System;
using System.Collections.Generic;
using System.Text;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionFactoryBuilder
    {
        protected Schedule Schedule;
        protected ICauseAnarchy Action;
        protected IHandleTime Timer;
        protected bool IsActive;

        public MockAnarchyActionFactoryBuilder()
        {

        }

        public MockAnarchyActionFactoryBuilder(ICauseAnarchy action = null, IHandleTime timer = null)
        {
            Action = action ?? Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            Timer = timer ?? Substitute.For<IHandleTime>();
            Schedule = null;
            IsActive = false;
        }

        public MockAnarchyActionFactoryBuilder WithSchedule(Schedule schedule)
        {
            Schedule = schedule;
            return this;
        }

        public MockAnarchyActionFactoryBuilder WithAction(ICauseAnarchy action)
        {
            Action = action;
            return this;
        }

        public MockAnarchyActionFactoryBuilder WithIsActive(bool isActiveResponse)
        {
            IsActive = isActiveResponse;
            return this;
        }

        public IAnarchyActionFactory Build()
        {
            var factory = Substitute.For<IAnarchyActionFactory>();
            factory.ExecutionSchedule.Returns(Schedule);
            factory.AnarchyAction.Returns(Action);
            factory.IsActive.Returns(IsActive);

            return factory;
        }
    }
}
