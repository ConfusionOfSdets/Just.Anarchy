using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class AnarchyActionNotFoundExceptionHandler : BaseExceptionHandler<AnarchyActionNotFoundException>
    {
        public AnarchyActionNotFoundExceptionHandler(ILogAdapter<AnarchyActionNotFoundException> logger) : base(logger, "anarchy-action-not-found", StatusCodes.Status404NotFound)
        { }
    }
}
