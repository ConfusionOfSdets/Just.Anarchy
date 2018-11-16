using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class UnrequestableActionExceptionHandler : BaseExceptionHandler<UnrequestableActionException>
    {
        public UnrequestableActionExceptionHandler() : base("action-type-does-not-handle-requests", StatusCodes.Status400BadRequest)
        { }
    }
}
