using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class MultipleResponseAlteringActionsEnabledExceptionHandler : BaseExceptionHandler<MultipleResponseAlteringActionsEnabledException>
    {
        public MultipleResponseAlteringActionsEnabledExceptionHandler() : base("too-many-response-altering-anarchy-actions-enabled", StatusCodes.Status400BadRequest)
        { }
    }
}
