using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class AnarchyActionNotFoundExceptionHandler : BaseExceptionHandler<AnarchyActionNotFoundException>
    {
        public AnarchyActionNotFoundExceptionHandler() : base("anarchy-action-not-found", StatusCodes.Status404NotFound)
        { }
    }
}
