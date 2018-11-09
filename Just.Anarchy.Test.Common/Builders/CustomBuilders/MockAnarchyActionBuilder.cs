using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionBuilder
    {
        private bool _schedulable = true;
        private string _name = "testAction";

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

        public ICauseAnarchy Build() => _schedulable ? BuildSchedulable() : BuildUnschedulable();
        
        private ICauseAnarchy BuildSchedulable()
        {
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            action.Name.Returns(_name);
            action.AnarchyType.Returns(CauseAnarchyType.Passive);
            action.Active.Returns(true);
            return action;
        }

        private ICauseAnarchy BuildUnschedulable()
        {
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns(_name);
            action.AnarchyType.Returns(CauseAnarchyType.Passive);
            action.Active.Returns(true);
            return action;
        }
    }
}
