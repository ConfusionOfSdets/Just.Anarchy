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
        public void MatchingFactory_ValidNewSchedule(string anarchyType)
        {
            //arrange
            var schedule = new Schedule();
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { factory });

            //act
            sut.AssignScheduleToAnarchyActionFactory(anarchyType, schedule, false);

            //assert
            factory.Received(1).AssociateSchedule(schedule);
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingFactory(string anarchyType)
        {
            //arrange
            var schedule = new Schedule();
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { factory });

            //act
            var exception = Assert.Catch(() => sut.AssignScheduleToAnarchyActionFactory(anarchyType, schedule, false));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            factory.Received(0).AssociateSchedule(Arg.Any<Schedule>());
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingFactory(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var schedule = new Schedule();
            var factory1 = Substitute.For<IAnarchyActionFactory>();
            var factory2 = Substitute.For<IAnarchyActionFactory>();
            var firstAction = Substitute.For<ICauseAnarchy>();
            firstAction.Name.Returns(firstActionName);
            var secondAction = Substitute.For<ICauseAnarchy>();
            secondAction.Name.Returns(secondActionName);
            factory1.AnarchyAction.Returns(firstAction);
            factory2.AnarchyAction.Returns(secondAction);
            var sut = new AnarchyManagerNew(new [] { factory1, factory2 });

            //act
            sut.AssignScheduleToAnarchyActionFactory("firstActionName", schedule, false);

            //assert
            factory1.Received(firstCount).AssociateSchedule(Arg.Any<Schedule>());
            factory2.Received(secondCount).AssociateSchedule(Arg.Any<Schedule>());
        }

        [Test]
        public void MatchingFactory_ScheduleExists_DoNotAllowUpdate()
        {
            //arrange
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.ExecutionSchedule.Returns(new Schedule());
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new [] { factory });
            var schedule = new Schedule();

            //act
            var exception = Assert.Catch(() => sut.AssignScheduleToAnarchyActionFactory("testAnarchyType", new Schedule(), false));

            //assert
            exception.Should().BeOfType<ScheduleExistsException>();
            factory.Received(0).AssociateSchedule(schedule);
        }

        [Test]
        public void MatchingFactory_ScheduleExists_AllowUpdate()
        {
            //arrange
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.ExecutionSchedule.Returns(new Schedule());
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new[] { factory });
            var schedule = new Schedule();

            //act
            sut.AssignScheduleToAnarchyActionFactory("testAnarchyType", schedule, true);

            //assert
            factory.Received(1).AssociateSchedule(schedule);
        }
    }
}
