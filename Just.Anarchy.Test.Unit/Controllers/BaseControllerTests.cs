using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Just.Anarchy.Test.Unit.Controllers
{
    public abstract class BaseControllerTests
    {
        protected T ControllerWithContextBuilder<T>(Func<T> controllerCreatorFunction) where T : Controller
        {
            var controller = controllerCreatorFunction();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            return controller;
        }
    }
}
