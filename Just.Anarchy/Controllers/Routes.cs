using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Controllers
{
    public static class Routes
    {
        private const string AnarchyBase = "/anarchy";

        public static class Anarchy
        {
            public const string Trigger = AnarchyBase + "/{anarchyType}/trigger";
            public const string SetOrCancelOnRequestHandling = AnarchyBase + "/{anarchyType}/onrequest";
            public const string UpdateAction = AnarchyBase + "/{anarchyType}";
            public const string StopAction = AnarchyBase + "/{anarchyType}/stop";
            public const string StopAllActions = AnarchyBase + "/stop";
        }

        public static class Schedule
        {
            public const string GetSetUpdate = AnarchyBase + "/{anarchyType}/schedule";
            public const string Start = GetSetUpdate + "/start";
            public const string StartAll = GetAll + "/start";
            public const string GetAll = AnarchyBase + "/schedules";
        }
    }
}
