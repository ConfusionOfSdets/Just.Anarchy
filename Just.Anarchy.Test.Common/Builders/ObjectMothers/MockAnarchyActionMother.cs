using Just.Anarchy.Test.Common.Builders.CustomBuilders;

namespace Just.Anarchy.Test.Common.Builders.ObjectMothers
{
    public class MockAnarchyActionMother
    {
        public MockAnarchyActionBuilder UnschedulableAction => new MockAnarchyActionBuilder().ThatIsUnschedulable();
        public MockAnarchyActionBuilder SchedulableAction => new MockAnarchyActionBuilder().ThatIsSchedulable();
    }
}
