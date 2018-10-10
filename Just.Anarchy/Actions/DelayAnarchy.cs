using System;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy.Actions
{
    public class DelayAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public bool Active { get; set; } = false;

        public int StatusCode => 0;

        public string Body => "";
        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            var random = new Random();
            var randomSecs = random.Next(1, 60);
            return Task.Delay(TimeSpan.FromSeconds(randomSecs), cancellationToken);
        }
    }
}