using System;
using Just.Anarchy.Test.Common.Builders.CustomBuilders;

namespace Just.Anarchy.Test.Common.Builders.ObjectMothers
{
    public class MockAnarchyActionOrchestratorMother
    {
        public MockAnarchyActionOrchestratorBuilder OrchestratorWithSchedule => new MockAnarchyActionOrchestratorBuilder()
                                                                    .WithSchedule(Get.CustomBuilderFor.Schedule);

        public Func<string, MockAnarchyActionOrchestratorBuilder> OrchestratorWithScheduleNamed => named =>
            new MockAnarchyActionOrchestratorBuilder()
                .WithSchedule(Get.CustomBuilderFor.Schedule)
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                    .Named(named)
                    .Build());

        public MockAnarchyActionOrchestratorBuilder OrchestratorWithoutSchedule => new MockAnarchyActionOrchestratorBuilder();

        public Func<string, MockAnarchyActionOrchestratorBuilder> OrchestratorWithoutScheduleNamed => named =>
            new MockAnarchyActionOrchestratorBuilder()
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                    .Named(named)
                    .Build());

        public MockAnarchyActionOrchestratorBuilder OrchestratorWithUnschedulableAction => new MockAnarchyActionOrchestratorBuilder()
            .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                .ThatIsUnschedulable()
                .Build());

        public Func<string, MockAnarchyActionOrchestratorBuilder> OrchestratorWithUnschedulableActionNamed => named =>
            new MockAnarchyActionOrchestratorBuilder()
                .WithAction(Get.CustomBuilderFor.MockAnarchyAction
                    .Named(named)
                    .ThatIsUnschedulable()
                    .Build());

    }
}
