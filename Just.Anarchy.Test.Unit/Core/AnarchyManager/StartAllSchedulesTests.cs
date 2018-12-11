using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class StartAllSchedulesTests
    {
        [Test]
        public void OnlySchedulableActionsAreStarted()
        {
            //arrange
            var orchestrators = new[]
            {
                Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithSchedule.Build(),
                Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithUnschedulableAction.Build(),
                Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithSchedule.Build(),
            };
            
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            sut.StartAllSchedules();

            //assert
            foreach (var orchestrator in orchestrators)
            {
                if (orchestrator.AnarchyAction is ICauseScheduledAnarchy)
                {
                    orchestrator.Received(1).Start();
                }
                else
                {
                    orchestrator.DidNotReceive().Start();
                }
            }
        }

        [Test]
        public void NoOrchestrators()
        {
            //arrange
            var sut = new AnarchyManagerNew(new IActionOrchestrator[] {  });

            //act/assert
            Assert.DoesNotThrow(() => sut.StartAllSchedules());
        }

        [Test]
        public void NoScheduledOrchestrators()
        {
            //arrange
            var orchestrators = new[]
            {
                Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithUnschedulableAction.Build(),
                Get.MotherFor.MockAnarchyActionOrchestrator.OrchestratorWithUnschedulableAction.Build()
            };

            var sut = new AnarchyManagerNew(orchestrators);

            //act
            sut.StartAllSchedules();

            //assert
            foreach (var orchestrator in orchestrators)
            {
                orchestrator.DidNotReceive().Start();
            }
        }
    }
}
