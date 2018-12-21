using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class InvalidTargetPatternExceptionHandler : BaseExceptionHandler<InvalidTargetPatternException>
    {
        public InvalidTargetPatternExceptionHandler(ILogAdapter<InvalidTargetPatternException> logger) : base(logger, "invalid-target-pattern-specified", StatusCodes.Status400BadRequest)
        {
            
        }
    }
}
