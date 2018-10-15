using System;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy
{
    public class AnarchyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAnarchyManager _chaosManager;
        private readonly IHandleAnarchyExceptions _exceptionHandler;

        public AnarchyMiddleware(RequestDelegate next, IAnarchyManager chaosManager, IHandleAnarchyExceptions exceptionHandler)
        {
            _next = next;
            _chaosManager = chaosManager;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path.Contains("/anarchy/schedule/"))
            {
                try
                {
                    await _next(context);
                }
                catch (Exception e)
                {
                    await _exceptionHandler.HandleAsync(context.Response, e);
                }
            }

            if(_chaosManager.GetState() == AnarchyState.Active && !context.Request.Path.Value.Contains("status/anarchy"))
            {
                var action = _chaosManager.ChooseRandomAnarchyActionFactory();

                try
                {
                    action.HandleRequest(context.Request.Path);
                }
                catch (Exception e)
                {
                    await _exceptionHandler.HandleAsync(context.Response, e);
                }

                if (action.AnarchyAction.AnarchyType == CauseAnarchyType.Passive)
                {   
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = action.AnarchyAction.StatusCode;
                    await context.Response.WriteAsync(action.AnarchyAction.Body);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
