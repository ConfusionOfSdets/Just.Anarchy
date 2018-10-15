using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Unit.utils;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core
{
    [TestFixture]
    public class AnarchyManagerTests
    {
        [Test]
        [TestCase("TESTAnarchyType")]
        [TestCase("testAnarchyType")]
        public void AssignScheduleToAnarchyAction_MatchingFactory_ValidSchedule(string anarchyType)
        {
            //arrange
            var schedule = new Schedule();
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new IAnarchyActionFactory[] { factory });

            //act
            sut.AssignScheduleToAnarchyActionFactory(anarchyType, schedule);

            //assert
            factory.Received(1).AssociateSchedule(schedule);
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void AssignScheduleToAnarchyAction_NoMatchingFactory(string anarchyType)
        {
            //arrange
            var schedule = new Schedule();
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new IAnarchyActionFactory[] { factory });

            //act
            var exception = Assert.Catch(() => sut.AssignScheduleToAnarchyActionFactory(anarchyType, schedule));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            factory.Received(0).AssociateSchedule(Arg.Any<Schedule>());
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void AssignScheduleToAnarchyAction_SelectsFirstMatchingFactory(string firstActionName, string secondActionName, int firstCount, int secondCount)
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
            var sut = new AnarchyManagerNew(new IAnarchyActionFactory[] { factory1, factory2 });

            //act
            sut.AssignScheduleToAnarchyActionFactory("firstActionName", schedule);

            //assert
            factory1.Received(firstCount).AssociateSchedule(Arg.Any<Schedule>());
            factory2.Received(secondCount).AssociateSchedule(Arg.Any<Schedule>());
        }

        [Test]
        public void AssignScheduleToAnarchyAction_MatchingFactory_ScheduleExists()
        {
            //arrange
            var factory = Substitute.For<IAnarchyActionFactory>();
            var action = Substitute.For<ICauseAnarchy>();
            action.Name.Returns("testAnarchyType");
            factory.ExecutionSchedule.Returns(new Schedule());
            factory.AnarchyAction.Returns(action);
            var sut = new AnarchyManagerNew(new IAnarchyActionFactory[] { factory });

            //act
            var exception = Assert.Catch(() => sut.AssignScheduleToAnarchyActionFactory("testAnarchyType", new Schedule()));

            //assert
            exception.Should().BeOfType<ScheduleExistsException>();
            factory.Received(0).AssociateSchedule(Arg.Any<Schedule>());
        }
    }
}
