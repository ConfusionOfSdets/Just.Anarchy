using System;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Actions
{
    public class DelayAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; } = CauseAnarchyType.Passive;
        public string Name => nameof(DelayAnarchy);

        public TimeSpan? DelayBefore { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan? DelayAfter { get; set; } = TimeSpan.FromSeconds(5);

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            try
            {
                if (DelayBefore.HasValue)
                {
                    await Task.Delay(DelayBefore.Value, cancellationToken);
                }

                next(context).Wait(cancellationToken);

                if (DelayAfter.HasValue)
                {
                    await Task.Delay(DelayAfter.Value, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Deliberately catch this as this is expected
            }
            catch (OperationCanceledException)
            {
                // Deliberately catch this as this is expected
            }
        }
    }
}