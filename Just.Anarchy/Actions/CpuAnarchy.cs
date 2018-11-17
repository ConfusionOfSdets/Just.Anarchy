using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Actions
{
    public class CpuAnarchy : ICauseScheduledAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public string Name => nameof(CpuAnarchy);
        public bool Active { get; set; } = false;

        public TimeSpan DefaultDuration { get; } = TimeSpan.FromMinutes(1);

        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    Enumerable
                        .Range(1,
                            Environment
                                .ProcessorCount) // replace with lesser number if 100% usage is not what you are after.
                        .AsParallel()
                        .WithCancellation(cancellationToken)
                        .Select(i =>
                        {
                            var end = DateTime.Now + (duration ?? DefaultDuration);
                            while (DateTime.Now < end && !cancellationToken.IsCancellationRequested)
                                /*nothing here */
                                ;
                            return i;
                        }).ToList(); // ToList makes the query execute.
                }
                catch (OperationCanceledException)
                {
                    // suppress the expected exception if cancelled
                }
                
            }, cancellationToken);

        }

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            await ExecuteAsync(DefaultDuration, cancellationToken);
        }
    }
}