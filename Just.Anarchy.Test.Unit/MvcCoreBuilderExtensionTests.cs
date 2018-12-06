using System;
using FluentAssertions;
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
    }
}
