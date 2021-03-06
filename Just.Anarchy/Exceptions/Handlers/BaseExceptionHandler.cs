﻿using System;
using System.Threading.Tasks;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Just.Anarchy.Exceptions.Handlers
{
    public abstract class BaseExceptionHandler<T> : IExceptionHandler where T:Exception
    {
        private readonly ILogAdapter<T> _logger;
        private readonly string _errorShortCode;
        private readonly int _statusCode;
        private readonly string _detail;

        protected BaseExceptionHandler(ILogAdapter<T> logger, string errorShortCode, int statusCode, string detail = null)
        {
            _logger = logger;
            _errorShortCode = errorShortCode;
            _statusCode = statusCode;
            _detail = detail;
        }

        public bool CanHandle(Exception e) => e is T;

        public virtual async Task HandleExceptionAsync(HttpResponse response, Exception e)
        {
            if (!CanHandle(e))
            {
                throw new ArgumentException("ExceptionHandler has been asked to handle exception of a type it does not handle!", e);
            }

            var payload = JsonConvert.SerializeObject(new ControllerErrorResult
            {
                Errors = new[] { _errorShortCode }
            });

            response.StatusCode = _statusCode;
            await response.WriteAsync(payload);
        }

        public virtual void LogException(Exception e)
        {
            if (!CanHandle(e))
            {
                throw new ArgumentException("ExceptionHandler has been asked to log exception of a type it does not handle!", e);
            }

            _logger.Error($"{_errorShortCode} - {e.Message}", e, _errorShortCode, e.Message);
        }
    }
}
