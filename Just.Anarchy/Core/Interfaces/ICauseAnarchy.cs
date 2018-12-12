using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Core.Interfaces
{
    public interface ICauseAnarchy
    {
        CauseAnarchyType AnarchyType { get; }
        string Name { get; }
        
        /// <summary>
        /// Execute one execution of the specified AnarchyAction acting upon a given HttpContext
        /// </summary>
        /// <param name="context">The HttpContext containing the request/response loop</param>
        /// <param name="cancellationToken">The cancellation token which if triggered will abort the http request</param>
        Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken);
    }
}