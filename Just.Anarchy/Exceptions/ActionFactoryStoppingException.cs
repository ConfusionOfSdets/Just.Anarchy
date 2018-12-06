using System;

namespace Just.Anarchy.Exceptions
{
    public class ActionStoppingException : Exception
    {
        public ActionStoppingException() : base("The ActionOrchestrator has been requested to stop, your request cannot be fulfilled.")
        {
        }
    }
}
