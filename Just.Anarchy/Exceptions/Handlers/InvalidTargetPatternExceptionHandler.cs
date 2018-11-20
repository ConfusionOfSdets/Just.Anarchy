using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class InvalidTargetPatternExceptionHandler : BaseExceptionHandler<InvalidTargetPatternException>
    {
        public InvalidTargetPatternExceptionHandler() : base("invalid-target-pattern-specified", StatusCodes.Status400BadRequest)
        {
            
        }
    }
}
