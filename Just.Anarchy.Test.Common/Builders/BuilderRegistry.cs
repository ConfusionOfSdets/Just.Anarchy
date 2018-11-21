using Just.Anarchy.Test.Common.Builders.CustomBuilders;

namespace Just.Anarchy.Test.Common.Builders
{
    public class BuilderRegistry
    {
        public MockAnarchyActionBuilder MockAnarchyAction => new MockAnarchyActionBuilder();
        public MockHttpContextBuilder MockHttpContext => new MockHttpContextBuilder();
        public MockAnarchyActionFactoryBuilder MockAnarchyActionFactory => new MockAnarchyActionFactoryBuilder();
        public MockAnarchyActionFactoriesBuilder MockAnarchyActionFactories => new MockAnarchyActionFactoriesBuilder();
        public ScheduleBuilder Schedule => new ScheduleBuilder();
    }
}
