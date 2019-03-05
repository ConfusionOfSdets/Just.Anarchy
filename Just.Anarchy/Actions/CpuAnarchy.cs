using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy.Actions
{
    public class CpuAnarchy : ICauseScheduledAnarchy
    {
        private readonly ILogger<CpuAnarchy> _logger;
        public CauseAnarchyType AnarchyType { get; } = CauseAnarchyType.Passive;
        public string Name => nameof(CpuAnarchy);

        public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromMinutes(1);
        public int CpuLoadPercentage { get; set; } = 50;

        public CpuAnarchy(ILogger<CpuAnarchy> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                KillCpuCoresForAWhile(CpuLoadPercentage, duration ?? DefaultDuration, cancellationToken);
                
            }, cancellationToken);
        }

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            #pragma warning disable 4014 // Intentionally not awaiting a response here as we want to respond before CPU Anarchy has completed
            ExecuteAsync(DefaultDuration, cancellationToken);
            #pragma warning restore 4014
            await next(context);
        }

        private void KillCpuCoresForAWhile(int cpuLoadPercentage, TimeSpan duration, CancellationToken parentCancellationToken)
        {
            _logger.LogInformation($"CPUAnarchy started at {DateTime.Now} (at: {cpuLoadPercentage}% for: {duration.TotalSeconds}s)");
            var timeoutCancellationToken = new CancellationTokenSource(duration).Token;
            
            // Cancel either based on time expiry or cancellation
            var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationToken, parentCancellationToken).Token;

            try
            {
                Enumerable
                    .Range(1, Environment.ProcessorCount) // spread load across all processors
                    .AsParallel()
                    .WithCancellation(cancellationToken)
                    .Select(i =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            KillCPUCore(cpuLoadPercentage, cancellationToken);
                        }
                        return i;
                    }).ToList(); // ToList makes the query execute.
            }
            catch (OperationCanceledException)
            {
                // suppress the expected exception if cancelled - we expect OperationCancelledException if cancels inside KillCpuCore
            }
            catch (AggregateException e)
            {
                // suppress the expected exception if cancelled - we expect AggregateException if cancelled while executing the Parallel portion of the code
                // check the inner exceptions for OperationCancelledExceptions, anything else should be thrown
                if (e.InnerExceptions.Any(i => i.GetType() != typeof(OperationCanceledException)))
                {
                    throw;
                }
            }
            _logger.LogInformation($"CPUAnarchy finished at {DateTime.Now}");
        }

        private void KillCPUCore(int cpuLoadPercentage, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            while (!cancellationToken.IsCancellationRequested && sw.ElapsedMilliseconds < 100)
            {                
                // This bit causes load by repeatedly spinning the while loop
                if (sw.ElapsedMilliseconds <= cpuLoadPercentage) continue;

                // This bit allows the loop to pause giving the CPU a break
                Task.Delay(100 - cpuLoadPercentage, cancellationToken).Wait(cancellationToken);
            }
            sw.Stop();
        }
    }
}