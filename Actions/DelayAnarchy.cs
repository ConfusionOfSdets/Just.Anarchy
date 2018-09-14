using System;
using System.Threading.Tasks;

namespace Just.Anarchy.Actions
{
    public class DelayAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.Passive;
        public bool Active { get; set; } = false;

        public int StatusCode => 0;

        public string Body => "";

        public async Task ExecuteAsync()
        {
            Random random = new Random();
            var randomSecs = random.Next(1, 60);
            await Task.Delay(TimeSpan.FromSeconds(randomSecs));
        }
    }
}