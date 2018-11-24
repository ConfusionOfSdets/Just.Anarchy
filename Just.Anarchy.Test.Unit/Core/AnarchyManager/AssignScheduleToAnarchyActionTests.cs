using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class AssignScheduleToAnarchyActionTests
    {
        [Test]
        [TestCase("TESTAnarchyType")]
        [TestCase("testAnarchyType")]
        public void MatchingOrchestrator_ValidNewSchedule(string anarchyType)
        {
            //arrange
            var schedule = new Schedule();
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            sut.AssignScheduleToActionOrchestrator(anarchyType, schedule, false);

            //assert
            orchestrator.Received(1).AssociateSchedule(schedule);
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
            var exception = Assert.Catch(() => sut.AssignScheduleToActionOrchestrator(anarchyType, schedule, false));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            orchestrator.Received(0).AssociateSchedule(Arg.Any<Schedule>());
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingOrchestrator(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var schedule = new Schedule();
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
            sut.AssignScheduleToActionOrchestrator("firstActionName", schedule, false);

            //assert
            orchestrator1.Received(firstCount).AssociateSchedule(Arg.Any<Schedule>());
            orchestrator2.Received(secondCount).AssociateSchedule(Arg.Any<Schedule>());
        }

        [Test]
        public void MatchingOrchestrator_ScheduleExists_DoNotAllowUpdate()
        {
            //arrange
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.ExecutionSchedule.Returns(new Schedule());
            orchestrator.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { orchestrator });
            var schedule = new Schedule();

            //act
            var exception = Assert.Catch(() => sut.AssignScheduleToActionOrchestrator("testAnarchyType", new Schedule(), false));

            //assert
            exception.Should().BeOfType<ScheduleExistsException>();
            orchestrator.Received(0).AssociateSchedule(schedule);
        }

        [Test]
        public void MatchingOrchestrator_ScheduleExists_AllowUpdate()
        {
            //arrange
            var orchestrator = Substitute.For<IActionOrchestrator>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            orchestrator.ExecutionSchedule.Returns(new Schedule());
            orchestrator.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new[] { orchestrator });
            var schedule = new Schedule();

            //act
            sut.AssignScheduleToActionOrchestrator("testAnarchyType", schedule, true);

            //assert
            orchestrator.Received(1).AssociateSchedule(schedule);
        }
    }
}
