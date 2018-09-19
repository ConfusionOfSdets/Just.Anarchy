using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Just.Anarchy.Actions
{
    public class MemoryAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public bool Active { get; set; } = false;

        public int StatusCode => 0;

        public string Body => string.Empty;

        public async Task ExecuteAsync()
        {
            var rnd = new Random();
            var list = new List<byte[]>();
            var limit = rnd.Next(1000, 1000000);
            var result = Parallel.ForEach(Enumerable.Range(1, limit),
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                num =>
                {
                    byte[] b = new byte[1024];
                    b[rnd.Next(0, b.Length)] = byte.MaxValue;
                    list.Add(b);
                }
            );
        }
    }
}