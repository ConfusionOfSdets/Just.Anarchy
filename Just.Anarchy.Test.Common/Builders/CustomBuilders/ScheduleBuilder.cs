using System;
using System.Collections.Generic;
using System.Text;
using Just.Anarchy.Core;
using Just.Anarchy.Test.Common.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class ScheduleBuilder : Schedule
    {

        public ScheduleBuilder()
        {
        }

        public ScheduleBuilder WithRandomValues()
        {
            return It.IsAny<ScheduleBuilder>();
        }
    }
}

