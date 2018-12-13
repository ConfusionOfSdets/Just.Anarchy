using System;
using FluentAssertions;
using Just.Anarchy.Test.Common.Fakes;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit
{
    [TestFixture]
    public class MvcCoreBuilderExtensionTests
    {
        [Test]
        public void AddAnarchy_WhenBuilderIsNullExceptionIsThrown()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchy(null));

            //Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddAnarchy_WhenBuilderIsNullExceptionContainsCorrectParameter()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchy(null));

            //Assert
            ((ArgumentNullException)exception).ParamName.Should().Be("builder");
        }

        [Test]
        public void AddAnarchyCore_WhenBuilderIsNullExceptionIsThrown()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchyCore(null));

            //Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddAnarchyCore_WhenBuilderIsNullExceptionContainsCorrectParameter()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchyCore(null));

            //Assert
            ((ArgumentNullException)exception).ParamName.Should().Be("builder");
        }

        [Test]
        public void AddAnarchyDefaultActions_WhenBuilderIsNullExceptionIsThrown()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchyDefaultActions(null));

            //Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddAnarchyDefaultActions_WhenBuilderIsNullExceptionContainsCorrectParameter()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchyDefaultActions(null));

            //Assert
            ((ArgumentNullException)exception).ParamName.Should().Be("builder");
        }

        [Test]
        public void AddAnarchyAction_WhenBuilderIsNullExceptionIsThrown()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchyAction<FakeAnarchyAction>(null));

            //Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddAnarchyAction_WhenBuilderIsNullExceptionContainsCorrectParameter()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.AddAnarchyAction<FakeAnarchyAction>(null));

            //Assert
            ((ArgumentNullException)exception).ParamName.Should().Be("services");
        }

        [Test]
        public void UseAnarchy_WhenBuilderIsNullExceptionIsThrown()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.UseAnarchy(null));

            //Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public void UseAnarchy_WhenBuilderIsNullExceptionContainsCorrectParameter()
        {
            // Act
            var exception = Assert.Catch(() => MvcCoreBuilderExtensions.UseAnarchy(null));

            //Assert
            ((ArgumentNullException)exception).ParamName.Should().Be("builder");
        }
    }
}
