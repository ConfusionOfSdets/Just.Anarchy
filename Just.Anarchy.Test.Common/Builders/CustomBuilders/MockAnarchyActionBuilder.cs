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
        private Action<CancellationToken> _execAction;
        private Action<CancellationToken> _handleRequestAction;

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

        public MockAnarchyActionBuilder ThatExecutesTask(Action<CancellationToken> execAction)
        {
            _execAction = execAction;
            return this;
        }

        public MockAnarchyActionBuilder ThatHandlesRequestWithTask(Action<CancellationToken> handleRequestAction)
        {
            _handleRequestAction = handleRequestAction;
            return this;
        }

        public ICauseAnarchy Build()
        {
            var action = _schedulable ? BuildSchedulable() : BuildUnschedulable();
            action.Name.Returns(_name);
            action.AnarchyType.Returns(CauseAnarchyType.Passive);
            action.Active.Returns(true);

            if (_handleRequestAction != null)
            {
                action
                    .HandleRequestAsync(Arg.Any<HttpContext>(), Arg.Any<RequestDelegate>(), Arg.Any<CancellationToken>())
                    .Returns(a => Task.Run(() => _handleRequestAction(a.ArgAt<CancellationToken>(1))));
            }

            return action;
        }

        private ICauseAnarchy BuildSchedulable()
        {
            var action = Substitute.For<ICauseScheduledAnarchy>();
            if (_execAction != null)
            {
                action
                    .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
                    .Returns(a => Task.Run(() => _execAction(a.ArgAt<CancellationToken>(1))));
            }

            return action;
        }

        private ICauseAnarchy BuildUnschedulable() => Substitute.For<ICauseAnarchy>();
    }
}
