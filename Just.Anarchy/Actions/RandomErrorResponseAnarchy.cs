using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Just.Anarchy.Actions
{
    public class RandomErrorResponseAnarchy : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; set; } = CauseAnarchyType.AlterResponse;
        public string Name => nameof(RandomErrorResponseAnarchy);
        public bool Active { get; set; } = false;

        public int StatusCode { get; set; }

        public string Body { get; set; }

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            var random = new Random();
            StatusCode = random.Next(400, 600);
            Body = JsonConvert.SerializeObject(RandomObject(cancellationToken));
            await context.Response.WriteAsync(Body, cancellationToken);
        }

        private static Dictionary<string,object> RandomObject(CancellationToken cancellationToken)
        {
            var random = new Random();
            var obj = new Dictionary<string, object>();
            Parallel.ForEach(Enumerable.Range(0,1000)
                .AsParallel()
                .WithCancellation(cancellationToken), 
                x => {
                    obj.Add(RandomString(random.Next(1, 1000)), RandomString(random.Next(1, 1000)));
                }
                );
            return obj;
        }
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}