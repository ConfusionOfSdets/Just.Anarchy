using System;
using System.Linq;
using System.Threading.Tasks;

namespace Just.Anarchy.Actions
{
    public class CpuAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public bool Active { get; set; } = false;

        public int StatusCode => 0;

        public string Body => string.Empty;

        public Task ExecuteAsync()
        {
            Random random = new Random();
            var randomSecs = random.Next(1, 20);
            Enumerable
                .Range(1, Environment.ProcessorCount) // replace with lesser number if 100% usage is not what you are after.
                .AsParallel()
                .Select(i =>
                {
                    var end = DateTime.Now + TimeSpan.FromSeconds(randomSecs);
                    while (DateTime.Now < end)
                        /*nothing here */
                        ;
                    return i;
                }).ToList(); // ToList makes the query execute.
            return Task.FromResult("");
        }
    }
}