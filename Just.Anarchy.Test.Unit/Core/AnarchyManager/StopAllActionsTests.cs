using Just.Anarchy.Core;
using Just.Anarchy.Test.Common.Builders;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class StopAllActionsTests
    {
        [Test]
        public void CallsAllOrchestrators()
        {
            //arrange
            var orchestrator1 = Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithoutSchedule.Build();
            var orchestrator2 = Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithSchedule.Build();

            var sut = new AnarchyManagerNew(new [] { orchestrator1, orchestrator2 });

            //act
            sut.StopAllActions();

            //assert
            orchestrator1.Received(1).Stop();
            orchestrator2.Received(1).Stop();
        }
    }
}
