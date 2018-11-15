using System;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;

namespace Just.Anarchy.Actions
{
    public class DelayAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public string Name => nameof(DelayAnarchy);
        public bool Active { get; set; } = false;

        public int StatusCode => 0;

        public string Body => "";

        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            return Task.Delay(duration ?? TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}