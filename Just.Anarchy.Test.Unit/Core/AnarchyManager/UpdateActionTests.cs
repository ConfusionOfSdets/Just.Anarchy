using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class UpdateActionTests
    {
        private const string FakeUpdatePayload = "afakepayload";

        [Test]
        [TestCase("TESTAnarchyType")]
        [TestCase("testAnarchyType")]
        public void MatchingOrchestrator(string anarchyType)
        {
            //arrange
            var orchestrator = Get.MotherFor.MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAnarchyType").Build();
            var sut = new AnarchyManagerNew(new [] { orchestrator });


            //act
            sut.UpdateAction(anarchyType, FakeUpdatePayload);

            //assert
            orchestrator.Received(1).UpdateAction(FakeUpdatePayload);
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingOrchestrator(string anarchyType)
        {
            //arrange
            var schedule = new Schedule();
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            var exception = Assert.Catch(() => sut.UpdateAction(anarchyType, FakeUpdatePayload));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            orchestrator.Received(0).UpdateAction(Arg.Any<string>());
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingOrchestrator(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var orchestrator1 = Get.MotherFor
                .MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed(firstActionName).Build();
            var orchestrator2 = Get.MotherFor
                .MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed(secondActionName).Build();
            var sut = new AnarchyManagerNew(new [] { orchestrator1, orchestrator2 });

            //act
            sut.UpdateAction("firstActionName", FakeUpdatePayload);

            //assert
            orchestrator1.Received(firstCount).UpdateAction(FakeUpdatePayload);
            orchestrator2.Received(secondCount).UpdateAction(FakeUpdatePayload);
        }
    }
}
