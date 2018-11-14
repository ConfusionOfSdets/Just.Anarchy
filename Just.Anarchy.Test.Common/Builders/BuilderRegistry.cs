using System;
using System.Collections.Generic;
using System.Text;
using Just.Anarchy.Test.Common.Builders.CustomBuilders;

namespace Just.Anarchy.Test.Common.Builders
{
    public class BuilderRegistry
    {
        public MockAnarchyActionBuilder MockAnarchyAction { get; } = new MockAnarchyActionBuilder();
        public MockAnarchyActionFactoryBuilder MockAnarchyActionFactory { get; } = new MockAnarchyActionFactoryBuilder();
        public MockAnarchyActionFactoriesBuilder MockAnarchyActionFactories { get; } = new MockAnarchyActionFactoriesBuilder();
        public ScheduleBuilder Schedule { get; } = new ScheduleBuilder();
    }
}
