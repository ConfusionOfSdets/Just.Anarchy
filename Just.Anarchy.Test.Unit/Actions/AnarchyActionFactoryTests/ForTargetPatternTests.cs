using System;
using System.Threading;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.AnarchyActionFactoryTests
{
    [TestFixture]
    public class ForTargetPatternTests
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
    }
}
