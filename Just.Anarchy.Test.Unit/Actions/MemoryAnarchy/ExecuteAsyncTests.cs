using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.MemoryAnarchy
{
    [TestFixture]
    public class ExecuteAsyncTests
    {
        [Test]
        public async Task ExecuteAsyncFinishesAfterSpecifiedDuration()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var expectedDuration = TimeSpan.FromSeconds(1);
            var mockLogger = Substitute.For<ILogger<Anarchy.Actions.MemoryAnarchy>>();
            var sut = new Anarchy.Actions.MemoryAnarchy(mockLogger) { AmountMb = 1 };

            //Act
            var sw = Stopwatch.StartNew();
            await sut.ExecuteAsync(expectedDuration, ctFromTest);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(expectedDuration, 500);
        }

        [Test]
        public async Task ExecuteAsyncStopsIfTokenCancelled()
        {
            //Arrange
            var duration = TimeSpan.FromSeconds(60);
            var mockLogger = Substitute.For<ILogger<Anarchy.Actions.MemoryAnarchy>>();
            var sut = new Anarchy.Actions.MemoryAnarchy(mockLogger) { AmountMb = 1 };
            var cancellationDuration = TimeSpan.FromSeconds(1);

            //Act
            var sw = Stopwatch.StartNew();
            await sut.ExecuteAsync(duration, new CancellationTokenSource(cancellationDuration).Token);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(cancellationDuration, 500);
        }
    }
}
