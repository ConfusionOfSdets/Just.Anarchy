using System;
using System.Threading;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.AnarchyActionFactoryTests
{
    [TestFixture]
    public class HandleRequestTests
    {
        [Test]
        public void HandleRequestRejectsIfTargetPatternDoesNotMatch()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern("/bob$");

            //Act
            sut.HandleRequest("/jim");

            //Assert
            action.DidNotReceive().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>()); ;
        }

        [Test]
        public void HandleRequestHandlesIfTargetPatternMatches()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern("/bob$");

            //Act
            sut.HandleRequest("/bob");

            //Assert
            action.Received().ExecuteAsync(Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>()); ;
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
    }
}
