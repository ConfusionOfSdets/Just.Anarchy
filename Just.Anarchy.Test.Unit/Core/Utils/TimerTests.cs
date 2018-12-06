using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Test.Common.Builders;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Timer = Just.Anarchy.Core.Utils.Timer;

namespace Just.Anarchy.Test.Unit.Core.Utils
{
    [TestFixture]
    public class TimerTests
    {
        [Test]
        public async Task DelayInitialWaitsForDurationCorrectly()
        {
            // Arrange
            var schedule = Get.CustomBuilderFor.Schedule.ToStartWithDelay(TimeSpan.FromMilliseconds(400));
            var sut = new Timer();

            // Act
            var sw = Stopwatch.StartNew();
            await sut.DelayInitial(schedule, CancellationToken.None);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(400),50);
        }

        [Test]
        public void DelayInitialCancelsIfTriggered()
        {
            // Arrange
            var schedule = Get.CustomBuilderFor.Schedule.ToStartWithDelay(TimeSpan.FromMilliseconds(1000));
            var sut = new Timer();
            
            // Act
            var sw = Stopwatch.StartNew();
            var cts = new CancellationTokenSource(150);
            Assert.CatchAsync(async () => await sut.DelayInitial(schedule, cts.Token));
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(150), 50);
        }

        [Test]
        public async Task DelayIntervalWaitsForDurationCorrectly()
        {
            // Arrange
            var schedule = Get.CustomBuilderFor.Schedule.WithInterval(TimeSpan.FromMilliseconds(500));
            var sut = new Timer();

            // Act
            var sw = Stopwatch.StartNew();
            await sut.DelayInterval(schedule, CancellationToken.None);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(500), 50);
        }

        [Test]
        public void DelayIntervalCancelsIfTriggered()
        {
            // Arrange
            var schedule = Get.CustomBuilderFor.Schedule.WithInterval(TimeSpan.FromMilliseconds(1000));
            var sut = new Timer();

            // Act
            var sw = Stopwatch.StartNew();
            var cts = new CancellationTokenSource(150);
            Assert.CatchAsync(async () => await sut.DelayInterval(schedule, cts.Token));
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(150));
        }
    }
}
