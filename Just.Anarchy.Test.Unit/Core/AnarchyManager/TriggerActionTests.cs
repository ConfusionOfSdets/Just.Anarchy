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
        public void MatchingOrchestrator(string anarchyType, int? durationSecs)
        {
            //arrange
            var orchestrator = Get.MotherFor
                .MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAnarchyType")
                .Build();

            var duration = durationSecs.HasValue ?
                TimeSpan.FromSeconds(durationSecs.Value) : 
                (TimeSpan?) null;

            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            sut.TriggerAction(anarchyType, duration);

            //assert
            orchestrator.Received(1).TriggerOnce(duration);
        }

        [Test]
        [TestCase(null)]
        [TestCase("    ")]
        [TestCase("")]
        [TestCase("wontMatch")]
        public void NoMatchingOrchestrator(string anarchyType)
        {
            //arrange
            var orchestrator = Get.MotherFor
                .MockAnarchyActionOrchestrator
                .OrchestratorWithScheduleNamed("testAnarchyType")
                .Build();

            var sut = new AnarchyManagerNew(new [] { orchestrator });

            //act
            var exception = Assert.Catch(() => sut.TriggerAction(anarchyType, null));

            //assert
            exception.Should().BeOfType<AnarchyActionNotFoundException>();
            orchestrator.Received(0).TriggerOnce(Arg.Any<TimeSpan?>());
        }

        [Test]
        [TestCase("firstActionName", "secondActionName", 1, 0)]
        [TestCase("secondActionName", "firstActionName", 0, 1)]
        [TestCase("firstActionName", "firstActionName", 1, 0)]
        public void SelectsFirstMatchingOrchestrator(string firstActionName, string secondActionName, int firstCount, int secondCount)
        {
            //arrange
            var schedule = new Schedule();

            var orchestrators = Get.CustomBuilderFor.MockAnarchyActionOrchestrators
                .WithActionsNamed(firstActionName, secondActionName)
                .WithSchedules(schedule, schedule)
                .Build()
                .Select(orchestratorBuilder => orchestratorBuilder.Build())
                .ToList();

            var sut = new AnarchyManagerNew(orchestrators);

            //act
            sut.TriggerAction("firstActionName", TimeSpan.FromSeconds(1));

            //assert
            orchestrators[0].Received(firstCount).TriggerOnce(Arg.Any<TimeSpan?>());
            orchestrators[1].Received(secondCount).TriggerOnce(Arg.Any<TimeSpan?>());
        }
    }
}
