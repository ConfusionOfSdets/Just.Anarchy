using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class StartScheduleTests
    {
        [Test]
        [TestCase("TESTAnarchyType")]
        [TestCase("testAnarchyType")]
        public void MatchingOrchestrator_ValidNewSchedule(string anarchyType)
        {
            //arrange
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            sut.StartSchedule(anarchyType);

            //assert
            orchestrator.Received(1).Start();
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingOrchestrator(string anarchyType)
        {
            //arrange
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            var exception = Assert.Catch(() => sut.StartSchedule(anarchyType));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            orchestrator.Received(0).Start();
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingOrchestrator(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var orchestrator1 = Substitute.For<IActionOrchestrator>();
            var orchestrator2 = Substitute.For<IActionOrchestrator>();
            var firstAction = Substitute.For<ICauseAnarchy>();
            firstAction.Name.Returns(firstActionName);
            var secondAction = Substitute.For<ICauseAnarchy>();
            secondAction.Name.Returns(secondActionName);
            orchestrator1.AnarchyAction.Returns(firstAction);
            orchestrator2.AnarchyAction.Returns(secondAction);
            var sut = new AnarchyManagerNew(new [] { orchestrator1, orchestrator2 });

            //act
            sut.StartSchedule("firstActionName");

            //assert
            orchestrator1.Received(firstCount).Start();
            orchestrator2.Received(secondCount).Start();
        }
    }
}
