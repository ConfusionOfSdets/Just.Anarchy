using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Just.Anarchy.Actions
{
    public class MemoryAnarchy : ICauseScheduledAnarchy
    {
        private readonly ILogger<MemoryAnarchy> _logger;
        public CauseAnarchyType AnarchyType { get; } = CauseAnarchyType.Passive;
        public string Name => nameof(MemoryAnarchy);
        public int AmountMb { get; set; } = 1024;
        public TimeSpan DefaultDuration { get; } = TimeSpan.FromMinutes(1);
        
        //private const int Megabyte = 1048576;
        private const int Megabyte = 1000000;

        public MemoryAnarchy(ILogger<MemoryAnarchy> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            return ConsumeMemoryFor(duration ?? DefaultDuration, cancellationToken);
        }

        private async Task ConsumeMemoryFor(TimeSpan duration, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"MemoryAnarchy started at {DateTime.Now} (at: {AmountMb}Mb for: {duration.TotalSeconds}s)");

            var totalSize = Megabyte * AmountMb;

            // Consume memory quickly - Unmanaged seems most consistent
            var pointer = Marshal.AllocHGlobal(Marshal.SizeOf<byte>() * totalSize);

            try
            {
                Parallel.For(0, totalSize,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    i => Marshal.WriteByte(pointer, i, new byte())
                );

                // Hold the memory for the allotted duration (or the default duration, 1 min)
                await Task.Delay(duration, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Deliberately catch TaskCancelledExceptions
                _logger.LogInformation($"MemoryAnarchy cancelled at {DateTime.Now}");
            }
            finally
            {
                // Release Memory
                Marshal.FreeHGlobal(pointer);
                _logger.LogInformation($"MemoryAnarchy finished at {DateTime.Now}");
            }
        }

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            #pragma warning disable 4014 // Intentionally not awaiting a response here as we want to respond before Memory Anarchy has completed
            ExecuteAsync(DefaultDuration, cancellationToken);
            #pragma warning restore 4014
            await next(context);
        }
    }
}