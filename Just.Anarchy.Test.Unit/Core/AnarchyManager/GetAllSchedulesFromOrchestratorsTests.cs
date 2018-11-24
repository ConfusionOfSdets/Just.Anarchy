using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class GetAllSchedulesFromActionOrchestratorsTests
    {
        [Test]
        public void PopulatesScheduleNameCorrectly()
        {
            //arrange
            var orchestrators = GetTestActionOrchestratorsOfType(1, 1, 0);
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.ToArray()[0].Name.Should().Be(orchestrators[0].AnarchyAction.Name);
            result.ToArray()[1].Name.Should().Be(orchestrators[1].AnarchyAction.Name);
        }

        [Test]
        public void PopulatesScheduleCorrectly()
        {
            //arrange
            var orchestrators = GetTestActionOrchestratorsOfType(1, 1, 0);
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.ToArray()[0].Schedule.Should().Be(orchestrators[0].ExecutionSchedule);
            result.ToArray()[1].Schedule.Should().Be(orchestrators[1].ExecutionSchedule);
        }

        [Test]
        public void ReturnsSchedulableActionOrchestrators()
        {
            //arrange
            var orchestrators = GetTestActionOrchestratorsOfType(1, 0, 0);
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.Count().Should().Be(1);
        }

        [Test]
        public void ReturnsSchedulableButEmptyActionOrchestrators()
        {
            //arrange
            var orchestrators = GetTestActionOrchestratorsOfType(0, 1, 0);
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.Count().Should().Be(1);
        }

        [Test]
        public void DoesNotReturnUnschedulableActionOrchestrators()
        {
            //arrange
            var orchestrators = GetTestActionOrchestratorsOfType(0, 0, 1);
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.Should().BeEmpty();
        }

        [Test]
        [TestCase(1,1,1,2)]
        [TestCase(0,0,1,0)]
        public void OnlyReturnsSchedulableActionOrchestrators(int scheduled, int scheduledButEmpty, int unscheduled, int expectedCount)
        {
            //arrange
            var orchestrators = GetTestActionOrchestratorsOfType(scheduled, scheduledButEmpty, unscheduled);
            var sut = new AnarchyManagerNew(orchestrators);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.Count().Should().Be(expectedCount);
        }

        [Test]
        public void ReturnsCorrectlyWithoutActions()
        {
            //arrange
            var sut = new AnarchyManagerNew(new IActionOrchestrator[0]);

            //act
            var result = sut.GetAllSchedulesFromOrchestrators();

            //assert
            result.Count().Should().Be(0);
        }
        
        private IList<IActionOrchestrator> GetTestActionOrchestratorsOfType(
            int schedulable, 
            int schedulableEmpty,
            int unschedulable)
        {
            var orchestrators = new List<IActionOrchestrator>();
            
            for (var i = 0; i < schedulable; i++)
            {
                var orchestrator = Substitute.For<IActionOrchestrator>();
                var anarchyAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
                anarchyAction.Name.Returns($"scheduled-{i}");
                orchestrator.AnarchyAction.Returns(anarchyAction);
                orchestrator.ExecutionSchedule.Returns(new Schedule { RepeatCount = 1 });
                orchestrators.Add(orchestrator);
            }

            for (var i = 0; i < schedulableEmpty; i++)
            {
                var orchestrator = Substitute.For<IActionOrchestrator>();
                var anarchyAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
                anarchyAction.Name.Returns($"scheduledButEmpty-{i}");
                orchestrator.AnarchyAction.Returns(anarchyAction);
                orchestrator.ExecutionSchedule.Returns((Schedule)null);
                orchestrators.Add(orchestrator);
            }

            for (var i = 0; i < unschedulable; i++)
            {
                var orchestrator = Substitute.For<IActionOrchestrator>();
                var anarchyAction = Substitute.For<ICauseAnarchy>();
                anarchyAction.Name.Returns($"unschedulable-{i}");
                orchestrator.AnarchyAction.Returns(anarchyAction);
                orchestrator.ExecutionSchedule.Returns((Schedule)null);
                orchestrators.Add(orchestrator);
            }

            return orchestrators;
        }
    }
}
