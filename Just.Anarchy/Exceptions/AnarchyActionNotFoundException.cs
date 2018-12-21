using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Exceptions
{
    public sealed class AnarchyActionNotFoundException : Exception
    {
        public AnarchyActionNotFoundException() : this(null)
        {
        }

        public AnarchyActionNotFoundException(string missingActionType) :
            base("The specified anarchy action does not exist.")
        {
            this.Data.Add("MissingAnarchyAction", missingActionType);
        }
    }
}
