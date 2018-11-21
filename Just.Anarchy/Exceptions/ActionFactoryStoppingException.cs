using System;

namespace Just.Anarchy.Exceptions
{
    public class ActionStoppingException : Exception
    {
        public ActionStoppingException() : base("The AnarchyActionFactory has been requested to stop, your request cannot be fulfilled.")
        {
        }
    }
}
