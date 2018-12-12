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

        private TimeSpan? _delayBefore;
        private TimeSpan? _delayAfter;

        public DelayAnarchy()
        {
            _delayBefore = TimeSpan.FromSeconds(5);
            _delayAfter = TimeSpan.FromSeconds(5);
        }

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            if (_delayBefore.HasValue)
            {
                await Task.Delay(_delayBefore.Value, cancellationToken);
            }
            
            next(context).Wait(cancellationToken);

            if (_delayAfter.HasValue)
            {
                await Task.Delay(_delayAfter.Value, cancellationToken);
            }
        }
    }
}