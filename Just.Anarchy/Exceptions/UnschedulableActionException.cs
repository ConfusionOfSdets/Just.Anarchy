﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Exceptions
{
    public class UnschedulableActionException : Exception
    {
        public UnschedulableActionException() : base("The ActionOrchestrator action specified is not able to be scheduled.")
        {
        }
    }
}
