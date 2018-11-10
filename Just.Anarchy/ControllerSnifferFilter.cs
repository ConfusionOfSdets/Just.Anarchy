using Microsoft.AspNetCore.Mvc.Filters;

namespace Just.Anarchy
{
    public class ControllerSnifferFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

            var model = context.ActionArguments["model"];
            
            // do something before the action executes
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}
