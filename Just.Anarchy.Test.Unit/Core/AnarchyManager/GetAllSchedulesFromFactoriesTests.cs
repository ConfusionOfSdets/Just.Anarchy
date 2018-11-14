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
    public class GetAllSchedulesFromFactoriesTests
    {
        [Test]
        public void PopulatesScheduleNameCorrectly()
        {
            //arrange
            var factories = GetTestFactoriesOfType(1, 1, 0);
            var sut = new AnarchyManagerNew(factories);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.ToArray()[0].Name.Should().Be(factories[0].AnarchyAction.Name);
            result.ToArray()[1].Name.Should().Be(factories[1].AnarchyAction.Name);
        }

        [Test]
        public void PopulatesScheduleCorrectly()
        {
            //arrange
            var factories = GetTestFactoriesOfType(1, 1, 0);
            var sut = new AnarchyManagerNew(factories);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.ToArray()[0].Schedule.Should().Be(factories[0].ExecutionSchedule);
            result.ToArray()[1].Schedule.Should().Be(factories[1].ExecutionSchedule);
        }

        [Test]
        public void ReturnsSchedulableActionFactories()
        {
            //arrange
            var factories = GetTestFactoriesOfType(1, 0, 0);
            var sut = new AnarchyManagerNew(factories);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.Count().Should().Be(1);
        }

        [Test]
        public void ReturnsSchedulableButEmptyActionFactories()
        {
            //arrange
            var factories = GetTestFactoriesOfType(0, 1, 0);
            var sut = new AnarchyManagerNew(factories);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.Count().Should().Be(1);
        }

        [Test]
        public void DoesNotReturnUnschedulableActionFactories()
        {
            //arrange
            var factories = GetTestFactoriesOfType(0, 0, 1);
            var sut = new AnarchyManagerNew(factories);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.Should().BeEmpty();
        }

        [Test]
        [TestCase(1,1,1,2)]
        [TestCase(0,0,1,0)]
        public void OnlyReturnsSchedulableActionFactories(int sched, int schedEmpty, int unsched, int expectedCount)
        {
            //arrange
            var factories = GetTestFactoriesOfType(sched, schedEmpty, unsched);
            var sut = new AnarchyManagerNew(factories);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.Count().Should().Be(expectedCount);
        }

        [Test]
        public void ReturnsCorrectlyWithoutActions()
        {
            //arrange
            var sut = new AnarchyManagerNew(new IAnarchyActionFactory[0]);

            //act
            var result = sut.GetAllSchedulesFromFactories();

            //assert
            result.Count().Should().Be(0);
        }
        
        private IList<IAnarchyActionFactory> GetTestFactoriesOfType(
            int schedulable, 
            int schedulableEmpty,
            int unschedulable)
        {
            var factories = new List<IAnarchyActionFactory>();
            
            for (var i = 0; i < schedulable; i++)
            {
                var factory = Substitute.For<IAnarchyActionFactory>();
                var anarchyAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
                anarchyAction.Name.Returns($"sched-{i}");
                factory.AnarchyAction.Returns(anarchyAction);
                factory.ExecutionSchedule.Returns(new Schedule { RepeatCount = 1 });
                factories.Add(factory);
            }

            for (var i = 0; i < schedulableEmpty; i++)
            {
                var factory = Substitute.For<IAnarchyActionFactory>();
                var anarchyAction = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
                anarchyAction.Name.Returns($"schedEmpty-{i}");
                factory.AnarchyAction.Returns(anarchyAction);
                factory.ExecutionSchedule.Returns((Schedule)null);
                factories.Add(factory);
            }

            for (var i = 0; i < unschedulable; i++)
            {
                var factory = Substitute.For<IAnarchyActionFactory>();
                var anarchyAction = Substitute.For<ICauseAnarchy>();
                anarchyAction.Name.Returns($"unschedulable-{i}");
                factory.AnarchyAction.Returns(anarchyAction);
                factory.ExecutionSchedule.Returns((Schedule)null);
                factories.Add(factory);
            }

            return factories;
        }
    }
}
