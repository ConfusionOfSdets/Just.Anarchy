using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Just.ContainedAnarchy
{
    public class ControllerSnifferFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

            context.ActionArguments["id"] = 1000;
            // do something before the action executes
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}
