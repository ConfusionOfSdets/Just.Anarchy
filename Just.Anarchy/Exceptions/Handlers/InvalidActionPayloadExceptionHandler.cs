using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class InvalidActionPayloadExceptionHandler : BaseExceptionHandler<InvalidActionPayloadException>
    {
        public InvalidActionPayloadExceptionHandler() : base("invalid-action-payload", StatusCodes.Status400BadRequest)
        { }
    }
}
