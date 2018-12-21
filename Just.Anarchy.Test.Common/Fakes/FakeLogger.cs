using System;
using System.Collections.Generic;
using Just.Anarchy.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy.Test.Common.Fakes
{
    public class FakeLogger<T> : ILogAdapter<T>
    {
        public IList<LogEvent<T>> LoggedEvents;

        public FakeLogger()
        {
            LoggedEvents = new List<LogEvent<T>>();
        }

        public void Error(string message)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Error,
                message = message
            });
        }

        public void Error(string messageTemplate, Exception exception)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Error,
                message = messageTemplate,
                Exception = exception
            });
        }

        public void Error(string messageTemplate, params object[] args)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Error,
                message = messageTemplate,
                args = args
            });
        }

        public void Error(string messageTemplate, Exception exception, params object[] args)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Error,
                message = messageTemplate,
                Exception = exception,
                args = args
            });
        }

        public void Info(string message)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Information,
                message = message
            });
        }

        public void Info(string messageTemplate, params object[] args)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Information,
                message = messageTemplate,
                args = args
            });
        }

        public void Warning(string message)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Warning,
                message = message
            });
        }

        public void Warning(string messageTemplate, params object[] args)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Warning,
                message = messageTemplate,
                args = args
            });
        }

        public void Debug(string message)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Debug,
                message = message,
            });
        }

        public void Debug(string messageTemplate, params object[] args)
        {
            LoggedEvents.Add(new LogEvent<T>
            {
                LogLevel = LogLevel.Debug,
                message = messageTemplate,
                args = args
            });
        }
    }

    public class LogEvent<T> {
        public LogLevel LogLevel { get; set; }
        public string message { get; set; }
        public object[] args { get; set; }
        public Exception Exception { get; set; }
    }
}
