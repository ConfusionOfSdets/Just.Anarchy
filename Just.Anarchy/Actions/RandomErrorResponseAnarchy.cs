using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Just.Anarchy.Actions
{
    public class RandomErrorResponseAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.AlterResponse;
        public bool Active { get; set; } = false;

        public int StatusCode { get; set; }

        public string Body { get; set; }

        public Task ExecuteAsync()
        {
            Random random = new Random();
            StatusCode = random.Next(400, 600);
            Body = JsonConvert.SerializeObject(RandomObject());
            return Task.FromResult(string.Empty);
        }

        private static Dictionary<string,object> RandomObject()
        {
            Random random = new Random();
            Dictionary<string, object> obj = new Dictionary<string, object>();
            Parallel.ForEach(Enumerable.Range(0,1000), x => {
                obj.Add(RandomString(random.Next(1, 1000)), RandomString(random.Next(1, 1000)));
            });
            return obj;
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}