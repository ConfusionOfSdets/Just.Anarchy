using System;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy
{
    public class AnarchyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAnarchyManagerNew _anarchyManager;
        private readonly IHandleAnarchyExceptions _exceptionHandler;

        public AnarchyMiddleware(RequestDelegate next, IAnarchyManagerNew anarchyManager, IHandleAnarchyExceptions exceptionHandler)
        {
            _next = next;
            _anarchyManager = anarchyManager;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            try
            {
                if (!path.StartsWith("/anarchy/"))
                {
                    await _anarchyManager.HandleRequest(context, _next);
                }
                else
                {
                    await _next(context);
                }
                
            }
            catch (Exception e)
            {
                await _exceptionHandler.HandleAsync(context.Response, e);
            }

            //if (path.Contains("/anarchy/schedule/"))
            //{
            //    try
            //    {
            //        await _next(context);
            //    }
            //    catch (Exception e)
            //    {
            //        await _exceptionHandler.HandleAsync(context.Response, e);
            //    }
            //}
            //if(_anarchyManager.GetState() == AnarchyState.Active && !context.Request.Path.Value.Contains("status/anarchy"))
            //{
            //    var action = _anarchyManager.ChooseRandomAnarchyActionFactory();

            //    try
            //    {
            //        action.HandleRequest(context.Request.Path);
            //    }
            //    catch (Exception e)
            //    {
            //        await _exceptionHandler.HandleAsync(context.Response, e);
            //    }

            //    if (action.AnarchyAction.AnarchyType == CauseAnarchyType.Passive)
            //    {   
            //        await _next(context);
            //    }
            //    else
            //    {
            //        context.Response.StatusCode = action.AnarchyAction.StatusCode;
            //        await context.Response.WriteAsync(action.AnarchyAction.Body);
            //    }
            //}
            //else
            //{
            //    await _next(context);
            //}
        }
    }
}
