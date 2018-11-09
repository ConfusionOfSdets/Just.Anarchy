using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;

namespace Just.Anarchy.Actions
{
    public class CpuAnarchy : ICauseAnarchy, ICauseScheduledAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public string Name => nameof(CpuAnarchy);
        public bool Active { get; set; } = false;

        public int StatusCode => 0;

        public string Body => string.Empty;
        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var random = new Random();
                var randomSecs = random.Next(1, 20);
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
                            var end = DateTime.Now + TimeSpan.FromSeconds(randomSecs);
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

        public Task ExecuteScheduledAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}