using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class EmptyTargetPatternExceptionHandler : BaseExceptionHandler<EmptyTargetPatternException>
    {
        public EmptyTargetPatternExceptionHandler() : base("empty-target-pattern-specified", StatusCodes.Status400BadRequest)
        { }
    }
}
