using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class InvalidActionPayloadExceptionHandler : BaseExceptionHandler<InvalidActionPayloadException>
    {
        public InvalidActionPayloadExceptionHandler(ILogAdapter<InvalidActionPayloadException> logger) : base(logger, "invalid-action-payload", StatusCodes.Status400BadRequest)
        { }
    }
}
