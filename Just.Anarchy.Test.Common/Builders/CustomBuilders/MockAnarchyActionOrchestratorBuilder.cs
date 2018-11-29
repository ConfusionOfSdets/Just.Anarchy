using System;
using System.Threading.Tasks;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionOrchestratorBuilder
    {
        protected Schedule Schedule;
        protected ICauseAnarchy Action;
        protected IHandleTime Timer;
        protected bool IsActive;
        protected bool CanHandleRequestResult;
        protected Func<HttpContext, RequestDelegate, Task> HandleRequestImplementation;

        public MockAnarchyActionOrchestratorBuilder()
        {

        }

        public MockAnarchyActionOrchestratorBuilder(ICauseAnarchy action = null, IHandleTime timer = null)
        {
            Action = action ?? Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            Timer = timer ?? Substitute.For<IHandleTime>();
            Schedule = null;
            IsActive = false;
        }

        public MockAnarchyActionOrchestratorBuilder WithSchedule(Schedule schedule)
        {
            Schedule = schedule;
            return this;
        }

        public MockAnarchyActionOrchestratorBuilder WithAction(ICauseAnarchy action)
        {
            Action = action;
            return this;
        }

        public MockAnarchyActionOrchestratorBuilder WithIsActive(bool isActiveResponse)
        {
            IsActive = isActiveResponse;
            return this;
        }

        public MockAnarchyActionOrchestratorBuilder ThatHasCanHandleResponse(bool canHandleResponse)
        {
            CanHandleRequestResult = canHandleResponse;
            return this;
        }

        public MockAnarchyActionOrchestratorBuilder ThatCanHandleRequest()
        {
            ThatHasCanHandleResponse(true);
            return this;
        }

        public MockAnarchyActionOrchestratorBuilder ThatCannotHandleRequest()
        {
            ThatHasCanHandleResponse(false);
            return this;
        }

        public MockAnarchyActionOrchestratorBuilder ThatCanHandleRequestWith(Func<HttpContext, RequestDelegate, Task> handleRequestImplementation)
        {
            ThatHasCanHandleResponse(true);
            HandleRequestImplementation = handleRequestImplementation;
            return this;
        }

        public IActionOrchestrator Build()
        {
            var orchestrator = Substitute.For<IActionOrchestrator>();
            orchestrator.ExecutionSchedule.Returns(Schedule);
            orchestrator.AnarchyAction.Returns(Action);
            orchestrator.IsActive.Returns(IsActive);
            orchestrator.CanHandleRequest(Arg.Any<string>()).Returns(CanHandleRequestResult);
            if (HandleRequestImplementation != null)
            {
                orchestrator.HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>()).Returns(a =>
                {
                    var context = a.ArgAt<HttpContext>(0);
                    var next = a.ArgAt<RequestDelegate>(1);
                    return HandleRequestImplementation(context, next);
                });
            }
            else
            {
                orchestrator.HandleRequest(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>()).Returns(Task.CompletedTask);
            }
            

            return orchestrator;
        }
    }
}
