using Just.Anarchy.Test.Common.Builders.CustomBuilders;

namespace Just.Anarchy.Test.Common.Builders
{
    public class BuilderRegistry
    {
        public MockAnarchyActionBuilder MockAnarchyAction => new MockAnarchyActionBuilder();
        public MockHttpContextBuilder MockHttpContext => new MockHttpContextBuilder();
        public MockAnarchyActionOrchestratorBuilder MockAnarchyActionOrchestrator => new MockAnarchyActionOrchestratorBuilder();
        public MockAnarchyActionOrchestratorsBuilder MockAnarchyActionOrchestrators => new MockAnarchyActionOrchestratorsBuilder();
        public ScheduleBuilder Schedule => new ScheduleBuilder();
        public MockSchedulerFactoryBuilder MockSchedulerFactory => new MockSchedulerFactoryBuilder();
    }
}
