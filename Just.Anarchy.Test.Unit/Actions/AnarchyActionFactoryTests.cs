using System;
using System.Threading;
using FluentAssertions;
using Just.Anarchy.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions
{
    [TestFixture]
    public class AnarchyActionFactoryTests
    {
        [Test]
        public void ForTargetPatternSetsTargetPatternIfNotWhitespace()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            var targetPattern = ".*";
            //Act
            sut.ForTargetPattern(targetPattern);

            //Assert
            sut.TargetPattern.Should().Be(targetPattern);
        }

        [Test]
        [TestCase("\t")]
        [TestCase("\r\n")]
        [TestCase("")]
        public void ForTargetPatternThrowsIfWhitespace(string targetPattern)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            Action forTargetPattern = () => sut.ForTargetPattern(targetPattern);

            //Act
            forTargetPattern.Should().Throw<ArgumentException>();
            
            //Assert
            action.DidNotReceive().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void ForTargetPatternNullDisablesHandleRequest()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern(null);

            //Act
            sut.HandleRequest(null);

            //Assert
            action.DidNotReceive().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>()); ;
        }

        [Test]
        [TestCase("/bob", true)]
        [TestCase("/jim", false)]
        public void HandleRequestRejectsIfTargetPatternDoesNotMatch(string url, bool expMatch)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern("/bob$");

            //Act
            sut.HandleRequest(url);

            //Assert
            if (expMatch) {
                action.Received().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>()); ;
            } else
            {
                action.DidNotReceive().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>()); ;
            }
        }

        [Test]
        [TestCase("/")]
        [TestCase("/bob")]
        [TestCase("")]
        public void HandleRequestRejectsAllUrlsIfTargetPatternIsNull(string url)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern(null);

            //Act
            sut.HandleRequest(url);
            
            //Assert
            action.DidNotReceive().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>()); ;
        }

        [Test]
        public void StartSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            //Act
            sut.Start();

            //Assert
            sut.IsActive.Should().BeTrue();
        }

        [Test]
        public void StopSetsIsActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            //Act
            sut.Stop();

            //Assert
            sut.IsActive.Should().BeFalse();
        }

        [Test]
        public void AssociateScheduleSetsScheduleCorrectly()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var schedule = new Schedule();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);

            //Act
            sut.AssociateSchedule(schedule);

            //Assert
            sut.ExecutionSchedule.Should().BeSameAs(schedule);
        }

        [Test]
        public void AssociateScheduleCannotSetScheduleWhenActive()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy, ICauseScheduledAnarchy>();
            var schedule = new Schedule();
            var timer = Substitute.For<IHandleTime>();
            var factory = new AnarchyActionFactory(action, timer);
            factory.Start();

            //Act
            Action sutCall = () => factory.AssociateSchedule(schedule);

            //Assert
            sutCall.Should().Throw<ScheduleRunningException>();
        }

        [Test]
        public void AssociateScheduleCannotSetScheduleForUnscheduledAction()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedule = new Schedule();
            var timer = Substitute.For<IHandleTime>();
            var factory = new AnarchyActionFactory(action, timer);

            //Act
            Action sutCall = () => factory.AssociateSchedule(schedule);

            //Assert
            sutCall.Should().Throw<UnschedulableActionException>();
        }
    }
}
