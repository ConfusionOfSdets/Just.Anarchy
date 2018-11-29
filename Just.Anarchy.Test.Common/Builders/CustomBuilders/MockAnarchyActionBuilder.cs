using System;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionBuilder
    {
        private bool _schedulable = true;
        private string _name = "testAction";
        private Func<CancellationToken, Task> _execTask;
        private Func<CancellationToken, Task> _handleRequestTask;
        private CauseAnarchyType _causeAnarchyType = CauseAnarchyType.Passive;

        public MockAnarchyActionBuilder()
        {
            
        }

        public MockAnarchyActionBuilder Named(string name)
        {
            _name = name;
            return this;
        }

        public MockAnarchyActionBuilder ThatIsUnschedulable()
        {
            _schedulable = false;
            return this;
        }

        public MockAnarchyActionBuilder ThatIsSchedulable()
        {
            _schedulable = true;
            return this;
        }

        public MockAnarchyActionBuilder ThatExecutesTask(Func<CancellationToken, Task> execTask)
        {
            _execTask = execTask;
            return this;
        }

        public MockAnarchyActionBuilder ThatHandlesRequestWithTask(Func<CancellationToken, Task> handleRequestTask)
        {
            _handleRequestTask = handleRequestTask;
            return this;
        }

        public MockAnarchyActionBuilder WithCauseAnarchyType(CauseAnarchyType causeAnarchyType)
        {
            _causeAnarchyType = causeAnarchyType;
            return this;
        }

        public ICauseAnarchy Build()
        {
            var action = _schedulable ? BuildSchedulable() : BuildUnschedulable();
            action.Name.Returns(_name);
            action.AnarchyType.Returns(_causeAnarchyType);

            if (_handleRequestTask != null)
            {
                action
                    .HandleRequestAsync(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>(), Arg.Any<CancellationToken>())
                    .Returns(a => Task.Run(async () => await _handleRequestTask(a.ArgAt<CancellationToken>(2))));
            }

            return action;
        }

        private ICauseAnarchy BuildSchedulable()
        {
            var action = Substitute.For<ICauseScheduledAnarchy>();
            if (_execTask != null)
            {
                action
                    .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
                    .Returns(a => Task.Run(async () => await _execTask(a.ArgAt<CancellationToken>(1))));
            }

            return action;
        }

        private ICauseAnarchy BuildUnschedulable() => Substitute.For<ICauseAnarchy>();
    }
}
