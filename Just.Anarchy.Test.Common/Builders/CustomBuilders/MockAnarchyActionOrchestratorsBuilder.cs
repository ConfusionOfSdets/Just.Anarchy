using System;
using Just.Anarchy.Core;
using System.Collections.Generic;
using System.Linq;
using Just.Anarchy.Core.Enums;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockAnarchyActionOrchestratorsBuilder
    {
        private Schedule[] _schedules;
        private string[] _actionNames;
        private CauseAnarchyType[] _causeAnarchyTypes;

        public MockAnarchyActionOrchestratorsBuilder()
        {
            _schedules = new Schedule[0];
            _actionNames = new string[0];
            _causeAnarchyTypes = new CauseAnarchyType[0];
        }

        public MockAnarchyActionOrchestratorsBuilder WithActionsNamed(params string[] actionNames)
        {
            _actionNames = actionNames;
            return this;
        }

        public MockAnarchyActionOrchestratorsBuilder WithSchedules(params Schedule[] schedules)
        {
            _schedules = schedules;
            return this;
        }

        public MockAnarchyActionOrchestratorsBuilder WithCauseAnarchyTypes(params CauseAnarchyType[] causeAnarchyTypes)
        {
            _causeAnarchyTypes = causeAnarchyTypes;
            return this;
        }

        public IList<MockAnarchyActionOrchestratorBuilder> Build()
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
                throw new Exception("AnarchyActionOrchestratorsBuildFailed - you need to supply the same number of schedules as action names!");
            }

            var builders = new List<MockAnarchyActionOrchestratorBuilder>();

            for (var i = 0; i < _schedules.Length; i++)
            {
                var schedule = _schedules[i];
                var actionName = _actionNames[i];
                var causeAnarchyType = _causeAnarchyTypes[i];

                var action = Get.CustomBuilderFor.MockAnarchyAction
                    .Named(actionName)
                    .WithCauseAnarchyType(causeAnarchyType)
                    .Build();

                var builder = Get.CustomBuilderFor.MockAnarchyActionOrchestrator
                    .WithAction(action)
                    .WithSchedule(schedule);
                    
                builders.Add(builder);
            }

            return builders;
        }
    }
}