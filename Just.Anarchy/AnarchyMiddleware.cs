using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy
{
    public class AnarchyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAnarchyManager _chaosManager;

        public AnarchyMiddleware(RequestDelegate next, IAnarchyManager chaosManager)
        {
            _next = next;
            _chaosManager = chaosManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if(_chaosManager.GetState() == AnarchyState.Active && !context.Request.Path.Value.Contains("status/anarchy"))
            {
                var action = _chaosManager.ChooseRandomAnarchyActionFactory();
                action.HandleRequest(context.Request.Path);
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
