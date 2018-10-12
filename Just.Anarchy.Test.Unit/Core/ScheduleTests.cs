using System;
using FluentAssertions;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core
{
    [TestFixture]
    public class ScheduleTests
    {
        [Test]
        public void ConstructorSetsDefaultDelayCorrectly()
        {
            //Act
            var sut = new Schedule();

            //Assert
            sut.Delay.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void ConstructorSetsDefaultIntervalCorrectly()
        {
            //Act
            var sut = new Schedule();

            //Assert
            sut.Interval.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void ConstructorSetsDefaultTotalDurationCorrectly()
        {
            //Act
            var sut = new Schedule();

            //Assert
            sut.TotalDuration.Should().Be(null);
        }

        [Test]
        public void ConstructorSetsDefaultIterationDurationCorrectly()
        {
            //Act
            var sut = new Schedule();

            //Assert
            sut.IterationDuration.Should().Be(null);
        }

        [Test]
        public void ConstructorSetsDefaultRepeatCountCorrectly()
        {
            //Act
            var sut = new Schedule();

            //Assert
            sut.RepeatCount.Should().Be(null);
        }

        [Test]
        [TestCase(1, null, null, null)]
        [TestCase(null, 1, null, null)]
        [TestCase(null, null, 1, null)]
        [TestCase(null, null, null, 1)]
        public void ConstructorWithValidParametersSetsTimespansCorrectly(int? delay, int? interval, int? iterationDuration, int? totalDuration)
        {
            //Arrange
            var delayParam = delay.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(delay.Value) : null;
            var intervalParam = interval.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(interval.Value) : null;
            var iterationDurationParam = iterationDuration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(iterationDuration.Value) : null;
            var totalDurationParam = totalDuration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(totalDuration.Value) : null;

            //Act
            var sut = new Schedule(delayParam, intervalParam, totalDurationParam, iterationDurationParam);


            //Assert
            sut.Delay.Should().Be(delayParam ?? TimeSpan.Zero);
            sut.Interval.Should().Be(intervalParam ?? TimeSpan.Zero);
            sut.TotalDuration.Should().Be(totalDurationParam);
            sut.IterationDuration.Should().Be(iterationDurationParam);
        }

        [Test]
        [TestCase(-1, null, null, null)]
        [TestCase(null, -1, null, null)]
        [TestCase(null, null, -1, null)]
        [TestCase(null, null, null, -1)]
        public void ConstructorWithNegativeTimespans(int? delay, int? interval, int? iterationDuration, int? totalDuration)
        {
            //Arrange
            var delayParam = delay.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(delay.Value) : null;
            var intervalParam = interval.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(interval.Value) : null;
            var iterationDurationParam = iterationDuration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(iterationDuration.Value) : null;
            var totalDurationParam = totalDuration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(totalDuration.Value) : null;

            //Act
            Action sutCall = () => new Schedule(delayParam, intervalParam, totalDurationParam, iterationDurationParam);

            //Assert
            sutCall.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ConstructorWithZeroRepeatCount()
        {
            //Act
            var sut = new Schedule(repeatCount: 0);

            //Assert
            sut.RepeatCount.Should().Be(0);
        }

        [Test]
        public void ConstructorWithNegativeRepeatCount()
        {
            //Act
            Action sutCall = () => new Schedule(repeatCount: -1);

            //Assert
            sutCall.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ToStartImmediatelySetsDelayCorrectly()
        {
            //Arrange
            var sut = new Schedule(delay: TimeSpan.FromSeconds(1));
            //Act
            var result = sut.ToStartImmediately();
            //Assert
            result.Delay.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void ToStartImmediatelyReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule(delay: TimeSpan.FromSeconds(1));
            //Act
            var result = sut.ToStartImmediately();
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void ToStartWithDelaySetsDelayCorrectly()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.ToStartWithDelay(TimeSpan.FromSeconds(5));
            //Assert
            result.Delay.Should().Be(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void ToStartWithDelayReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.ToStartWithDelay(TimeSpan.Zero);
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void WithIntervalSetsIntervalCorrectly()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.WithInterval(TimeSpan.FromSeconds(1));
            //Assert
            result.Interval.Should().Be(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void WithIntervalReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.WithInterval(TimeSpan.FromSeconds(1));
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void WithoutIntervalSetsIntervalCorrectly()
        {
            //Arrange
            var sut = new Schedule(interval: TimeSpan.FromSeconds(2));
            //Act
            var result = sut.WithoutInterval();
            //Assert
            result.Interval.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void WithoutIntervalReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule(interval: TimeSpan.FromSeconds(1));
            //Act
            var result = sut.WithoutInterval();
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void FinishAfterSetsTotalDurationCorrectly()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.FinishAfter(TimeSpan.FromSeconds(5));
            //Assert
            result.TotalDuration.Should().Be(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void FinishAfterReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.FinishAfter(TimeSpan.FromSeconds(5));
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void WithoutEndSetsTotalDurationCorrectly()
        {
            //Arrange
            var sut = new Schedule(totalDuration: TimeSpan.FromSeconds(3));
            //Act
            var result = sut.WithoutEnd();
            //Assert
            result.TotalDuration.Should().Be(null);
        }

        [Test]
        public void WithoutEndSetsRepeatCountCorrectly()
        {
            //Arrange
            var sut = new Schedule(totalDuration: TimeSpan.FromSeconds(3));
            //Act
            var result = sut.WithoutEnd();
            //Assert
            result.RepeatCount.Should().Be(0);
        }

        [Test]
        public void WithoutEndReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.WithoutEnd();
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void ForSetsIterationDurationCorrectly()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.For(TimeSpan.FromSeconds(4));
            //Assert
            result.IterationDuration.Should().Be(TimeSpan.FromSeconds(4));
        }

        [Test]
        public void ForReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.For(TimeSpan.FromSeconds(4));
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void ValidRepeatValueSetsRepeatCountCorrectlyZero()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.Repeat(0);
            //Assert
            result.RepeatCount.Should().Be(0);
        }

        [Test]
        public void ValidRepeatValueSetsRepeatCountCorrectlyPositiveInt()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.Repeat(1);
            //Assert
            result.RepeatCount.Should().Be(1);
        }

        [Test]
        public void ValidRepeatValueSetsRepeatCountCorrectlyMaxInt()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.Repeat(int.MaxValue);
            //Assert
            result.RepeatCount.Should().Be(int.MaxValue);
        }

        [Test]
        public void RepeatReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.For(TimeSpan.FromSeconds(4));
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void OnceSetsRepeatCountCorrectly()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.Once();
            //Assert
            result.RepeatCount.Should().Be(1);
        }

        [Test]
        public void OnceReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.Once();
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void NeverRepeatSetsRepeatCountCorrectly()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.NeverRepeat();
            //Assert
            result.RepeatCount.Should().Be(null);
        }

        [Test]
        public void NeverRepeatReturnsNewCopyOfSchedule()
        {
            //Arrange
            var sut = new Schedule();
            //Act
            var result = sut.NeverRepeat();
            //Assert
            result.Should().NotBeSameAs(sut);
        }

        [Test]
        public void WithoutEndRepeatsForever()
        {
            //Arrange
            var sut = new Schedule().WithoutEnd();
            //Act
            var result = sut.RepeatsForever;
            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ZeroRepeatCountRepeatsForever()
        {
            //Arrange
            var sut = new Schedule().Repeat(0);
            //Act
            var result = sut.RepeatsForever;
            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void PositiveRepeatCountRepeatsForever()
        {
            //Arrange
            var sut = new Schedule().Repeat(1);
            //Act
            var result = sut.RepeatsForever;
            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void NeverRepeatCountRepeatsForever()
        {
            //Arrange
            var sut = new Schedule().NeverRepeat();
            //Act
            var result = sut.RepeatsForever;
            //Assert
            result.Should().BeFalse();
        }
    }
}


