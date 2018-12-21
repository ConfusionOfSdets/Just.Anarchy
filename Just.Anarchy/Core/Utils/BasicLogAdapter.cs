using System;
using Just.Anarchy.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy.Core.Utils
{
    public class BasicLogAdapter<T> : ILogAdapter<T>
    {
        private ILogger<T> _logger;

        public BasicLogAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.LogError(exception, message);
        }

        public void Error(string messageTemplate, params object[] args)
        {
            _logger.LogError(messageTemplate, args);
        }

        public void Error(string messageTemplate, Exception exception, params object[] parameters)
        {
            _logger.LogError(exception, messageTemplate, parameters);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Info(string messageTemplate, params object[] args)
        {
            _logger.LogInformation(messageTemplate, args);
        }

        public void Warning(string message)
        {
            _logger.LogWarning(message);
        }

        public void Warning(string messageTemplate, params object[] args)
        {
            _logger.LogWarning(messageTemplate, args);
        }

        public void Debug(string message)
        {
            _logger.Log(LogLevel.Debug, null, message);
        }

        public void Debug(string messageTemplate, params object[] args)
        {
            _logger.LogDebug(messageTemplate, args);
        }
    }
}
