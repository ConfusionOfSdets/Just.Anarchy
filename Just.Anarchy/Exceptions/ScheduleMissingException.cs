using System;

namespace Just.Anarchy.Exceptions
{
    public class ScheduleMissingException : Exception
    {
        public ScheduleMissingException() : base("The ActionOrchestrator does not have a schedule set, this needs to be specified first.")
        {
        }
    }
}
