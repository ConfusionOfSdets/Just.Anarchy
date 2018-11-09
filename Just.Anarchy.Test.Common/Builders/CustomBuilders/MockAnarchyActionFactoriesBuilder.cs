using System;
using Just.Anarchy.Core;
using System.Collections.Generic;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionFactoriesBuilder
    {
        private Schedule[] _schedules;
        private string[] _actionNames;

        public MockAnarchyActionFactoriesBuilder()
        {
            _schedules = new Schedule[0];
            _actionNames = new string[0];
        }

        public MockAnarchyActionFactoriesBuilder WithFactoriesWithActionsNamed(params string[] actionNames)
        {
            _actionNames = actionNames;
            return this;
        }

        public MockAnarchyActionFactoriesBuilder WithFactoriesWithSchedules(params Schedule[] schedules)
        {
            _schedules = schedules;
            return this;
        }

        public IList<MockAnarchyActionFactoryBuilder> Build()
        {
            if (_schedules.Length != _actionNames.Length)
            {
                throw new Exception("AnarchyActionFactoriesBuildFailed - you need to supply the same number of schedules as action names!");
            }

            var builders = new List<MockAnarchyActionFactoryBuilder>();

            for (var i = 0; i < _schedules.Length; i++)
            {
                var schedule = _schedules[i];
                var actionName = _actionNames[i];

                var builder = Get.MotherFor.MockAnarchyActionFactory
                    .FactoryWithoutScheduleNamed(actionName)
                    .WithSchedule(schedule);

                builders.Add(builder);
            }

            return builders;
        }
    }
}