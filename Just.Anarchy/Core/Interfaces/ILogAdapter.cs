using System;

namespace Just.Anarchy.Core.Interfaces
{
    public interface ILogAdapter<T>
    {
        void Error(string message);
        void Error(string messageTemplate, Exception exception);
        void Error(string messageTemplate, params object[] args);
        void Error(string messageTemplate, Exception exception, params object[] args);
        void Info(string message);
        void Info(string messageTemplate, params object[] args);
        void Warning(string message);
        void Warning(string messageTemplate, params object[] args);
        void Debug(string message);
        void Debug(string messageTemplate, params object[] args);
    }
}
