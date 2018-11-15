using System;
using System.Linq;
using FluentAssertions;
using Just.Anarchy.Core;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Test.Common.Builders;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Core.AnarchyManager
{
    [TestFixture]
    public class TriggerActionTests
    {
        [Test]
        [TestCase("TESTAnarchyTyPe", null)]
        [TestCase("TESTAnarchyTyPe", 1)]
        [TestCase("testAnarchyType", 10)]
        public void MatchingFactory(string anarchyType, int? durationSecs)
        {
            //arrange
            var factory = Get.MotherFor
                .MockAnarchyActionFactory
                .FactoryWithScheduleNamed("testAnarchyType")
                .Build();

            var duration = durationSecs.HasValue ?
                TimeSpan.FromSeconds(durationSecs.Value) : 
                (TimeSpan?) null;

            var sut = new AnarchyManagerNew(new [] { factory });

            //act
            sut.TriggerAction(anarchyType, duration);

            //assert
            factory.Received(1).TriggerOnce(duration);
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingFactory(string anarchyType)
        {
            //arrange
            var factory = Get.MotherFor
                .MockAnarchyActionFactory
                .FactoryWithScheduleNamed("testAnarchyType")
                .Build();

            var sut = new AnarchyManagerNew(new [] { factory });

            //act
            var exception = Assert.Catch(() => sut.TriggerAction(anarchyType, null));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            factory.Received(0).TriggerOnce(Arg.Any<TimeSpan?>());
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingFactory(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var schedule = new Schedule();

            var factories = Get.CustomBuilderFor.MockAnarchyActionFactories
                .WithFactoriesWithActionsNamed(firstActionName, secondActionName)
                .WithFactoriesWithSchedules(schedule, schedule)
                .Build()
                .Select(factoryBuilder => factoryBuilder.Build())
                .ToList();

            var sut = new AnarchyManagerNew(factories);

            //act
            sut.TriggerAction("firstActionName", TimeSpan.FromSeconds(1));

            //assert
            factories[0].Received(firstCount).TriggerOnce(Arg.Any<TimeSpan?>());
            factories[1].Received(secondCount).TriggerOnce(Arg.Any<TimeSpan?>());
        }
    }
}
