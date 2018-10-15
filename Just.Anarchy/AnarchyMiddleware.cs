﻿using System;
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
            if(_chaosManager.GetState() == AnarchyState.Active && !context.Request.Path.Value.Contains("status/anarchy"))
            {
                var action = _chaosManager.ChooseRandomAnarchyActionFactory();

                await HandleRequestSafely(action, context);

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

        private async Task HandleRequestSafely(IAnarchyActionFactory actionFactory, HttpContext context)
        {
            try
            {
                actionFactory.HandleRequest(context.Request.Path);
            }
            catch (Exception e)
            {
                await _exceptionHandler.HandleAsync(context.Response, e);
            }
        }
    }
}
