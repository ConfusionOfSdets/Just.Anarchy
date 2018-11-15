using System;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionBuilder
    {
        private bool _schedulable = true;
        private string _name = "testAction";
        private Action<CancellationToken> _execAction;

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

        public ICauseAnarchy Build()
        {
            var action = _schedulable ? BuildSchedulable() : BuildUnschedulable();
            action.Name.Returns(_name);
            action.AnarchyType.Returns(CauseAnarchyType.Passive);
            action.Active.Returns(true);
            if (_execAction != null)
            {
                action
                    .ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
                    .Returns(a => Task.Run(() => _execAction(a.ArgAt<CancellationToken>(1))));
            }

            return action;
        }

        private ICauseAnarchy BuildSchedulable() => Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
        private ICauseAnarchy BuildUnschedulable() => Substitute.For<ICauseAnarchy>();
    }
}
