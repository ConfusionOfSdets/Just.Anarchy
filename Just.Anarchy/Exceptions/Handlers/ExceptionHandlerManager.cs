using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Exceptions.Handlers
{
    public class ExceptionHandlerManager : IHandleAnarchyExceptions
    {
        private readonly List<IExceptionHandler> _handlers;

        public ExceptionHandlerManager(IEnumerable<IExceptionHandler> handlers)
        {
            _handlers = handlers.ToList();
        }

        /// <summary>
        /// Handle all known Anarchy exceptions but throw if an unrecognised one appears
        /// </summary>
        /// <param name="response">The HttpResponse to return the error against</param>
        /// <param name="exception">The exception to handle</param>
        public async Task HandleAsync(HttpResponse response, Exception exception)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(exception));
            if (handler == null)
            {
                throw exception;
            }
            await handler.HandleExceptionAsync(response, exception);
            handler.LogException(exception);
        }
    }
}
