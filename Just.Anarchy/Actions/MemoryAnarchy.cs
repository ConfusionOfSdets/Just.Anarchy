using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;

namespace Just.Anarchy.Actions
{
    public class MemoryAnarchy : ICauseAnarchy, ICauseScheduledAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public string Name => nameof(MemoryAnarchy);
        public bool Active { get; set; } = false;
        public TimeSpan DefaultDuration { get; } = TimeSpan.FromMinutes(1);
        public int StatusCode => 0;

        // TODO: AmountMb needs to be parameterized.
        private const int AmountMb = 1024;
        private const int Megabyte = 1048576;

        public string Body => string.Empty;
        public Task ExecuteAsync(TimeSpan? duration, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var totalSize = Megabyte * AmountMb;

                // Consume memory quickly - Unmanaged seems most consistent
                var pointer = Marshal.AllocHGlobal(Marshal.SizeOf<byte>() * totalSize);
                Parallel.For(0, totalSize,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    i => Marshal.WriteByte(pointer, i, new byte())
                );

                // Hold the memory for the allotted duration (or 1 min)
                await Task.Delay(duration ?? DefaultDuration, cancellationToken);
                
                // Release Memory
                Marshal.FreeHGlobal(pointer);
            }, cancellationToken);
        }
    }
}