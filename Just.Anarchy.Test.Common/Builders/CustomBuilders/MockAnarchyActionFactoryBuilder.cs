using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Just.Anarchy.Actions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionFactoryBuilder
    {
        protected Schedule Schedule;
        protected ICauseAnarchy Action;
        protected IHandleTime Timer;
        protected bool IsActive;
        protected bool CanHandleRequestResult;

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

        public MockAnarchyActionFactoryBuilder ThatHasCanHandleResponse(bool canHandleResponse)
        {
            CanHandleRequestResult = canHandleResponse;
            return this;
        }

        public MockAnarchyActionFactoryBuilder ThatCanHandleRequest()
        {
            ThatHasCanHandleResponse(true);
            return this;
        }

        public MockAnarchyActionFactoryBuilder ThatCannotHandleRequest()
        {
            ThatHasCanHandleResponse(false);
            return this;
        }

        public IAnarchyActionFactory Build()
        {
            var factory = Substitute.For<IAnarchyActionFactory>();
            factory.ExecutionSchedule.Returns(Schedule);
            factory.AnarchyAction.Returns(Action);
            factory.IsActive.Returns(IsActive);
            factory.CanHandleRequest(Arg.Any<string>()).Returns(CanHandleRequestResult);
            factory.HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>()).Returns(Task.CompletedTask);

            return factory;
        }
    }
}
