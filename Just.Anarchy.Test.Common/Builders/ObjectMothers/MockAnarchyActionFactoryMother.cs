using System;
using Just.Anarchy.Test.Common.Builders.CustomBuilders;

namespace Just.Anarchy.Test.Common.Builders.ObjectMothers
{
    public class MockAnarchyActionFactoryMother
    {
        public MockAnarchyActionFactoryBuilder FactoryWithSchedule => new MockAnarchyActionFactoryBuilder()
                                                                    .WithSchedule(Get.CustomBuilderFor.Schedule);

        public Func<string, MockAnarchyActionFactoryBuilder> FactoryWithScheduleNamed => named =>
            new MockAnarchyActionFactoryBuilder()
                .WithSchedule(Get.CustomBuilderFor.Schedule)
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                    .Named(named)
                    .Build());

        public MockAnarchyActionFactoryBuilder FactoryWithoutSchedule => new MockAnarchyActionFactoryBuilder();

        public Func<string, MockAnarchyActionFactoryBuilder> FactoryWithoutScheduleNamed => named =>
            new MockAnarchyActionFactoryBuilder()
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                    .Named(named)
                    .Build());

        public MockAnarchyActionFactoryBuilder FactoryWithUnschedulableAction => new MockAnarchyActionFactoryBuilder()
            .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsUnschedulable()
                .Build());

        public Func<string, MockAnarchyActionFactoryBuilder> FactoryWithUnschedulableActionNamed => named =>
            new MockAnarchyActionFactoryBuilder()
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                    .Named(named)
                    .ThatIsUnschedulable()
                    .Build());

    }
}
