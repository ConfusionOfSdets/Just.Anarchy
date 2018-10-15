using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core.Interfaces
{
    public interface IHandleAnarchyExceptions
    {
        Task HandleAsync(HttpResponse response, Exception exception);
    }
}
