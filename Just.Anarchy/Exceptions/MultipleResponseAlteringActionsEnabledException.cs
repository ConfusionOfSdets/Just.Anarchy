using System;
using System.Collections;

namespace Just.Anarchy.Exceptions
{

    //TODO: ADD Handler for this exception!
    public sealed class MultipleResponseAlteringActionsEnabledException : Exception
    {
        public MultipleResponseAlteringActionsEnabledException() : this(null)
        {
        }

        public MultipleResponseAlteringActionsEnabledException(IEnumerable enabledActions) :
            base("There is more than 1 AnarchyAction enabled that is of type AlterResponse, this is not supported.")
        {
            this.Data.Add("EnabledResponseAlteringActions", enabledActions);
        }
    }
}
