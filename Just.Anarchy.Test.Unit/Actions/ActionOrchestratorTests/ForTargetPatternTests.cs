using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.ActionOrchestratorTests
{
    [TestFixture]
    public class ForTargetPatternTests
    {
        [Test]
        public void ForTargetPatternSetsTargetPatternIfNotWhitespace()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
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
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            Action forTargetPattern = () => sut.ForTargetPattern(targetPattern);

            //Act/Assert
            forTargetPattern.Should().Throw<EmptyTargetPatternException>();
        }

        [Test]
        [TestCase("*")]
        [TestCase("{-**?%")]
        public void ForTargetPatternThrowsIfNotValidRegex(string targetPattern)
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            Action forTargetPattern = () => sut.ForTargetPattern(targetPattern);

            //Act/Assert
            forTargetPattern.Should().Throw<InvalidTargetPatternException>();
        }

        [Test]
        public async Task ForTargetPatternNullDisablesHandleRequest()
        {
            //Arrange
            var action = Substitute.For<ICauseAnarchy>();
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<ICauseAnarchy>(action, schedulerFactory);
            sut.ForTargetPattern(null);

            //Act
            await sut.HandleRequest(context, next);

            //Assert
            await action.DidNotReceive().HandleRequestAsync(context, next, Arg.Any<CancellationToken>()); ;
        }
    }
}
