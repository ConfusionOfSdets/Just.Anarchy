using System;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit
{
    [TestFixture]
    public class AnarchyActionFactoryTests
    {
        [Test]
        [TestCase("/")]
        [TestCase("/bob")]
        [TestCase("")]
        public void HandleRequestRejectsAllUrlsIfTargetPatternIsNull(string url)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var sut = new AnarchyActionFactory(action);
            sut.ForTargetPattern(null);
            //Act
            sut.HandleRequest(url);
            //Assert
            action.DidNotReceive().ExecuteAsync();
        }

        [Test]
        [TestCase("\t")]
        [TestCase("\r\n")]
        [TestCase("")]
        public void ForTargetPatternRejectsInvalidTargetPatterns(string targetPattern)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var sut = new AnarchyActionFactory(action);
            Action setTargetPattern = () => sut.ForTargetPattern(targetPattern);

            //Act
            setTargetPattern.Should().Throw<ArgumentException>();
            
            //Assert
            action.DidNotReceive().ExecuteAsync();
        }

        [Test]
        public void ForTargetPatternNullDisablesHandleRequest()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var sut = new AnarchyActionFactory(action);
            sut.ForTargetPattern(null);

            //Act
            sut.HandleRequest(null);

            //Assert
            action.DidNotReceive().ExecuteAsync();
        }

        [Test]
        [TestCase("/bob", true)]
        [TestCase("/jim", false)]
        public void HandleRequestRejectsIfTargetPatternDoesNotMatch(string url, bool expMatch)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var sut = new AnarchyActionFactory(action);
            sut.ForTargetPattern("/bob$");

            //Act
            sut.HandleRequest(url);

            //Assert
            if (expMatch) {
                action.Received().ExecuteAsync();
            } else
            {
                action.DidNotReceive().ExecuteAsync();
            }
        }
    }
}
