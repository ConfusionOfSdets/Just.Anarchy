using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class GetScheduleFromAnarchyActionFactoryTests
    {
        [Test]
        public void MatchingFactory_ScheduleExists()
        {
            //arrange
            var schedule = new Schedule();
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            factory.ExecutionSchedule.Returns(schedule);
            var sut = new AnarchyManagerNew(new[] { factory });

            //act
            var result = sut.GetScheduleFromAnarchyActionFactory("testAnarchyType");

            //assert
            result.Should().Be(schedule);
        }

        [Test]
        public void MatchingFactory_ScheduleNotExists()
        {
            //arrange
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            factory.ExecutionSchedule.Returns((Schedule)null);
            var sut = new AnarchyManagerNew(new[] { factory });

            //act
            var result = sut.GetScheduleFromAnarchyActionFactory("testAnarchyType");

            //assert
            result.Should().BeNull();
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingFactory(string anarchyAction)
        {
            //arrange
            var schedule = new Schedule();
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            factory.ExecutionSchedule.Returns(schedule);
            var sut = new AnarchyManagerNew(new[] { factory });

            //act
            var exception = Assert.Catch(() => sut.GetScheduleFromAnarchyActionFactory(anarchyAction));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1)]
        [TestCase("secondActionName", "firstActionName", 2)]
        [TestCase("firstActionName", "firstActionName", 1)]
        public void SelectsFirstMatchingFactory(string firstActionName, string secondActionName, int repeatCount)
        {
            //arrange
            var factory1 = Substitute.For<IAnarchyActionFactory>();
            var factory2 = Substitute.For<IAnarchyActionFactory>();
            var firstAction = Substitute.For<ICauseAnarchy>();
            firstAction.Name.Returns(firstActionName);
            var secondAction = Substitute.For<ICauseAnarchy>();
            secondAction.Name.Returns(secondActionName);
            factory1.AnarchyAction.Returns(firstAction);
            factory1.ExecutionSchedule.Returns(new Schedule { RepeatCount = 1 });
            factory2.AnarchyAction.Returns(secondAction);
            factory2.ExecutionSchedule.Returns(new Schedule { RepeatCount = 2 });
            var sut = new AnarchyManagerNew(new[] { factory1, factory2 });

            //act
            var result = sut.GetScheduleFromAnarchyActionFactory("firstActionName");

            //assert
            result.RepeatCount.Should().Be(repeatCount);
        }
    }
}
