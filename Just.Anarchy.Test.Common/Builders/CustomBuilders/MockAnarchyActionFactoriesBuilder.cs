using System;
using Just.Anarchy.Core;
using System.Collections.Generic;
using System.Linq;
using Just.Anarchy.Core.Enums;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionFactoriesBuilder
    {
        private Schedule[] _schedules;
        private string[] _actionNames;
        private CauseAnarchyType[] _causeAnarchyTypes;

        public MockAnarchyActionFactoriesBuilder()
        {
            _schedules = new Schedule[0];
            _actionNames = new string[0];
            _causeAnarchyTypes = new CauseAnarchyType[0];
        }

        public MockAnarchyActionFactoriesBuilder WithActionsNamed(params string[] actionNames)
        {
            _actionNames = actionNames;
            return this;
        }

        public MockAnarchyActionFactoriesBuilder WithSchedules(params Schedule[] schedules)
        {
            _schedules = schedules;
            return this;
        }

        public MockAnarchyActionFactoriesBuilder WithCauseAnarchyTypes(params CauseAnarchyType[] causeAnarchyTypes)
        {
            _causeAnarchyTypes = causeAnarchyTypes;
            return this;
        }

        public IList<MockAnarchyActionFactoryBuilder> Build()
        {
            if (_schedules.Length == 0)
            {
                _schedules = _actionNames.Select(a => new Schedule()).ToArray();
            }

            if (_causeAnarchyTypes.Length == 0)
            {
                _causeAnarchyTypes = _actionNames.Select(a => CauseAnarchyType.Passive).ToArray();
            }

            if (_schedules.Length != _actionNames.Length)
            {
                throw new Exception("AnarchyActionFactoriesBuildFailed - you need to supply the same number of schedules as action names!");
            }

            var builders = new List<MockAnarchyActionFactoryBuilder>();

            for (var i = 0; i < _schedules.Length; i++)
            {
                var schedule = _schedules[i];
                var actionName = _actionNames[i];
                var causeAnarchyType = _causeAnarchyTypes[i];

                var action = Get.CustomBuilderFor.MockAnarchyAction
                    .Named(actionName)
                    .WithCauseAnarchyType(causeAnarchyType)
                    .Build();

                var builder = Get.CustomBuilderFor.MockAnarchyActionFactory
                    .WithAction(action)
                    .WithSchedule(schedule);
                    
                builders.Add(builder);
            }

            return builders;
        }
    }
}