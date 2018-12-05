using System.Collections.Generic;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Fakes;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.ActionOrchestratorTests
{
    [TestFixture]
    public class UpdateActionTests
    {
        
        [Test]
        public void UpdateActionValid_SinglePublicPropertyPayload_DoesNotAlterOtherProperties()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ \"ThisIsAPublicProperty\":\"Bob\" }";

            //Act
            sut.UpdateAction(payload);

            //Assert
            sut.AnarchyAction.Should().BeEquivalentTo(originalAction, e => e.Excluding(b => b.ThisIsAPublicProperty));
        }

        [Test]
        public void UpdateActionValid_SinglePublicPropertyPayload_SetsProperty()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ \"ThisIsAPublicProperty\":\"Bob\" }";

            //Act
            sut.UpdateAction(payload);

            //Assert
            ((FakeAnarchyAction)sut.AnarchyAction).ThisIsAPublicProperty.Should().Be("Bob");
        }

        [Test]
        public void UpdateActionValid_MultiplePublicPropertiesPayload_SetsProperties()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ \"ThisIsAPublicProperty\":\"Bob\", \"ThisIsAPublicDictionaryProperty\":{\"ThisIsAKey\":\"ThisIsAValue\"} }";

            //Act
            sut.UpdateAction(payload);

            //Assert
            var alteredAction = (FakeAnarchyAction) sut.AnarchyAction;
            alteredAction.ThisIsAPublicProperty.Should().Be("Bob");
            alteredAction.ThisIsAPublicDictionaryProperty.Should().BeEquivalentTo(new Dictionary<string,string>{{"ThisIsAKey", "ThisIsAValue"}});
        }

        [Test]
        public void UpdateActionValid_MultiplePublicPropertiesPayload_DoesNotAlterOtherProperties()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction { ThisIsAnIntegerProperty = 43 };
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ \"ThisIsAPublicProperty\":\"Bob\", \"ThisIsAPublicDictionaryProperty\":{\"ThisIsAKey\":\"ThisIsAValue\"} }";

            //Act
            sut.UpdateAction(payload);

            //Assert
            var alteredAction = (FakeAnarchyAction)sut.AnarchyAction;
            alteredAction.Should().BeEquivalentTo(originalAction, e => e
                .Excluding(b => b.ThisIsAPublicProperty)
                .Excluding(b => b.ThisIsAPublicDictionaryProperty));
        }

        [Test]
        [TestCase("{}")]
        [TestCase("{\t}")]
        public void UpdateActionValid_EmptyObjectPayload_DoesNotAlterAction(string payload)
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);

            //Act
            sut.UpdateAction(payload);

            //Assert
            sut.AnarchyAction.Should().BeEquivalentTo(originalAction);
        }

        [Test]
        public void UpdateActionValid_ReadonlyPropertyPayload_DoesNotAlterAction()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ 'Name':'ShouldNotBeSet' }";

            //Act
            sut.UpdateAction(payload);

            //Assert
            sut.AnarchyAction.Should().BeEquivalentTo(originalAction);
        }

        [Test]
        [TestCase("{ 'PropertyDoesntExist':'ShouldFail' }")]
        [TestCase("{ 'ThisIsAPublicProperty':'ShouldNotBeSetStill','PropertyDoesntExist':'ShouldFail' }")]
        public void UpdateActionInvalid_MissingMember_Throws(string payload)
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);

            //Act
            var exception = Assert.Catch(() => sut.UpdateAction(payload));

            //Assert
            exception.Should().BeOfType<InvalidActionPayloadException>();
        }

        [Test]
        [TestCase("{ 'PropertyDoesntExist':'ShouldFail' }")]
        [TestCase("{ 'ThisIsAPublicProperty':'ShouldNotBeSetStill','PropertyDoesntExist':'ShouldFail' }")]
        public void UpdateActionInvalid_MissingMember_DoesNotAlterAction(string payload)
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);

            //Act
            Assert.Catch(() => sut.UpdateAction(payload));

            //Assert
            sut.AnarchyAction.Should().BeEquivalentTo(originalAction);
        }

        [Test]
        [TestCase("")]
        [TestCase("/t")]
        [TestCase("{ 'ThisIsAPublicProperty':'PartialJson")]
        public void UpdateActionInvalid_MissingOrNonJsonPayload_Throws(string payload)
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);

            //Act
            var exception = Assert.Catch(() => sut.UpdateAction(payload));

            //Assert
            exception.Should().BeOfType<InvalidActionPayloadException>();
        }

        [Test]
        public void UpdateActionInvalid_NullForValueTypePayload_Throws()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ 'ThisIsAnIntegerProperty': null }";
            //Act
            var exception = Assert.Catch(() => sut.UpdateAction(payload));

            //Assert
            exception.Should().BeOfType<InvalidActionPayloadException>();
        }

        [Test]
        public void UpdateActionInvalid_NullForValueTypePayload_DoesNotAlterAction()
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);
            const string payload = "{ 'ThisIsAnIntegerProperty': null }";
            //Act
            Assert.Catch(() => sut.UpdateAction(payload));

            //Assert
            sut.AnarchyAction.Should().Be(originalAction);
        }

        [Test]
        [TestCase("")]
        [TestCase("/t")]
        [TestCase("{ 'ThisIsAPublicProperty':'PartialJson")]
        public void UpdateActionInvalid_MissingOrNonJsonPayload_DoesNotAlterAction(string payload)
        {
            //Arrange
            var originalAction = new FakeAnarchyAction();
            var schedulerFactory = Get.CustomBuilderFor.MockSchedulerFactory.Build();
            var sut = new ActionOrchestrator<FakeAnarchyAction>(originalAction, schedulerFactory);

            //Act
            Assert.Catch(() => sut.UpdateAction(payload));

            //Assert
            sut.AnarchyAction.Should().Be(originalAction);
        }
    }
}
