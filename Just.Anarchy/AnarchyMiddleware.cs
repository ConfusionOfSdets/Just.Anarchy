using System;
using System.Threading.Tasks;
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
                    var responseWritten = await _anarchyManager.HandleRequest(context, _next);

                    if (!responseWritten)
                    {
                        await _next(context);
                    }
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
        }
    }
}
