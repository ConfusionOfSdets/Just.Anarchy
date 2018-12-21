using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class EmptyTargetPatternExceptionHandler : BaseExceptionHandler<EmptyTargetPatternException>
    {
        public EmptyTargetPatternExceptionHandler(ILogAdapter<EmptyTargetPatternException> logger) : 
            base(logger, "empty-target-pattern-specified", StatusCodes.Status400BadRequest)
        { }
    }
}
