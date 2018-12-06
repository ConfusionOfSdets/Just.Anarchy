using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Requests;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.AnarchyController
{
    [TestFixture]
    public class SetActionTargetPatternTests
    {
        private const string anarchyType = "aFakeAnarchyType";
        

        [Test]
        public void SetActionTargetPatternTests_PassesTargetPatternToManager()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);
            var request = new EnableOnRequestHandlingRequest
            {
                TargetPattern = "aTargetPattern"
            };
            //Act
            sut.SetActionTargetPattern(anarchyType, request);

            //Assert
            anarchyManager.Received(1).AssignTargetPattern(anarchyType, request.TargetPattern);
        }

        [Test]
        public void SetActionTargetPatternTests_MissingPayload()
        {
            //Arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = new Anarchy.Controllers.AnarchyController(anarchyManager);
            //Act
            var exception = Assert.Catch(() => sut.SetActionTargetPattern(anarchyType, null));

            //Assert
            exception.Should().BeOfType<RequestBodyRequiredException<EnableOnRequestHandlingRequest>>();
        }
    }
}
