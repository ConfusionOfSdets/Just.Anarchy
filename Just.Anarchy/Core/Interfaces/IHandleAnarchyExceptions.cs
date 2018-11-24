using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core.Interfaces
{
    public interface IHandleAnarchyExceptions
    {
        Task HandleAsync(HttpResponse response, Exception exception);
    }
}
