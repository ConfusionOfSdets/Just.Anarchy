using System;

namespace Just.Anarchy.Exceptions
{
    public class UnrequestableActionException : Exception
    {
        public UnrequestableActionException() : base("The AnarchyActionFactory action specified is not able to handle http requests.")
        {
        }
    }
}
