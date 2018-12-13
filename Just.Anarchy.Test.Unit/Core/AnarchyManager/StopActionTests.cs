using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class StopActionTests
    {
        [Test]
        [TestCase("TESTAnarchyType")]
        [TestCase("testAnarchyType")]
        public void MatchingOrchestrator_ValidNewSchedule(string anarchyType)
        {
            //arrange
            var orchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAnarchyType").Build();
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            sut.StopAction(anarchyType);

            //assert
            orchestrator.Received(1).Stop();
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingOrchestrator(string anarchyType)
        {
            //arrange
            var orchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                    .OrchestratorWithScheduleNamed("testAnarchyType").Build();
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            var exception = Assert.Catch(() => sut.StopAction(anarchyType));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            orchestrator.Received(0).Stop();
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingOrchestrator(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var orchestrator1 = Get.MotherFor.MockAnarchyActionOrchestrator
                    .OrchestratorWithScheduleNamed(firstActionName).Build();
            var orchestrator2 = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed(secondActionName).Build();

            var sut = new AnarchyManagerNew(new [] { orchestrator1, orchestrator2 });

            //act
            sut.StopAction("firstActionName");

            //assert
            orchestrator1.Received(firstCount).Stop();
            orchestrator2.Received(secondCount).Stop();
        }
    }
}
