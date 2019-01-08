using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core.Interfaces
{
    public interface IExceptionHandler
    {
        bool CanHandle(Exception e);
        Task HandleExceptionAsync(HttpResponse context, Exception e);
        void LogException(Exception e);
    }
}
