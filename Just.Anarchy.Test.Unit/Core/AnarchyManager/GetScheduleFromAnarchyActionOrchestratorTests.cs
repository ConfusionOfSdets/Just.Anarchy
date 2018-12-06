using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class GetScheduleFromAnarchyActionOrchestratorTests
    {
        [Test]
        public void MatchingOrchestrator_ScheduleExists()
        {
            //arrange
            var schedule = new Schedule();
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            orchestrator.ExecutionSchedule.Returns(schedule);
            var sut = new AnarchyManagerNew(new[] { orchestrator });

            //act
            var result = sut.GetScheduleFromActionOrchestrator("testAnarchyType");

            //assert
            result.Should().Be(schedule);
        }

        [Test]
        public void MatchingOrchestrator_ScheduleNotExists()
        {
            //arrange
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            orchestrator.ExecutionSchedule.Returns((Schedule)null);
            var sut = new AnarchyManagerNew(new[] { orchestrator });

            //act
            var result = sut.GetScheduleFromActionOrchestrator("testAnarchyType");

            //assert
            result.Should().BeNull();
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingOrchestrator(string anarchyAction)
        {
            //arrange
            var schedule = new Schedule();
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            orchestrator.ExecutionSchedule.Returns(schedule);
            var sut = new AnarchyManagerNew(new[] { orchestrator });

            //act
            var exception = Assert.Catch(() => sut.GetScheduleFromActionOrchestrator(anarchyAction));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1)]
        [TestCase("secondActionName", "firstActionName", 2)]
        [TestCase("firstActionName", "firstActionName", 1)]
        public void SelectsFirstMatchingOrchestrator(string firstActionName, string secondActionName, int repeatCount)
        {
            //arrange
            var orchestrator1 = Substitute.For<IActionOrchestrator>();
            var orchestrator2 = Substitute.For<IActionOrchestrator>();
            var firstAction = Substitute.For<ICauseAnarchy>();
            firstAction.Name.Returns(firstActionName);
            var secondAction = Substitute.For<ICauseAnarchy>();
            secondAction.Name.Returns(secondActionName);
            orchestrator1.AnarchyAction.Returns(firstAction);
            orchestrator1.ExecutionSchedule.Returns(new Schedule { RepeatCount = 1 });
            orchestrator2.AnarchyAction.Returns(secondAction);
            orchestrator2.ExecutionSchedule.Returns(new Schedule { RepeatCount = 2 });
            var sut = new AnarchyManagerNew(new[] { orchestrator1, orchestrator2 });

            //act
            var result = sut.GetScheduleFromActionOrchestrator("firstActionName");

            //assert
            result.RepeatCount.Should().Be(repeatCount);
        }
    }
}
