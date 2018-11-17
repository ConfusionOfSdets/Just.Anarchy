using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Actions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Builders.CustomBuilders;
using Microsoft.AspNetCore.Http;
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

            //Act/Assert
            forTargetPattern.Should().Throw<ArgumentException>();
        }

        [Test]
        public async Task ForTargetPatternNullDisablesHandleRequest()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var timer = Substitute.For<IHandleTime>();
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            var sut = new AnarchyActionFactory(action, timer);
            sut.ForTargetPattern(null);

            //Act
            await sut.HandleRequest(context, next);

            //Assert
            await action.DidNotReceive().HandleRequestAsync(context, next, Arg.Any<CancellationToken>()); ;
        }
    }
}
