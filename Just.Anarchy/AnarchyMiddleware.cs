using System;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy
{
    public class AnarchyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAnarchyManagerNew _anarchyManager;
        private readonly IHandleAnarchyExceptions _exceptionHandler;
        private readonly ILogger<AnarchyMiddleware> _logger;

        public AnarchyMiddleware(RequestDelegate next, IAnarchyManagerNew anarchyManager, IHandleAnarchyExceptions exceptionHandler, ILogger<AnarchyMiddleware> logger)
        {
            _next = next;
            _anarchyManager = anarchyManager;
            _exceptionHandler = exceptionHandler;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            try
            {
                if (!path.StartsWith("/anarchy/"))
                {
                    _logger.LogInformation("Handling request '{path}' applying anarchy actions if applicable...", path);
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
