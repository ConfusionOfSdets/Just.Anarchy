using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Controllers
{
    public static class Routes
    {
        public static class Schedule
        {
            private const string ScheduleBase = "/anarchy/schedule";
            public const string GetSetUpdate = ScheduleBase + "/{anarchyType}";
            public const string GetAll = ScheduleBase;
        }
    }
}
