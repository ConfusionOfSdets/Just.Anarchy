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
        public CauseAnarchyType AnarchyType { get; } = CauseAnarchyType.AlterResponse;
        public string Name => nameof(RandomErrorResponseAnarchy);

        public int? StatusCode { get; set; }
        public string Body { get; set; }

        private static readonly Random Random = new Random();
        private static readonly object SyncLock = new object();

        public async Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            context.Response.StatusCode = StatusCode ?? GetRandomStatusCode();
            var bodyToWrite = string.IsNullOrEmpty(Body) ? JsonConvert.SerializeObject(RandomObject(cancellationToken)) : Body;

            await context.Response.WriteAsync(bodyToWrite, cancellationToken);
        }

        private static int GetRandomStatusCode() => RandomNumber(400, 600);

        private static Dictionary<string,object> RandomObject(CancellationToken cancellationToken)
        {
            var obj = new Dictionary<string, object>();
            Parallel.ForEach(Enumerable.Range(0,1000)
                .AsParallel()
                .WithCancellation(cancellationToken), 
                x => {
                    obj.Add(RandomString(RandomNumber(1, 1000)), RandomString(RandomNumber(1, 1000)));
                }
                );
            return obj;
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => chars[RandomNumber(chars.Length)]).ToArray());
        }

        private static int RandomNumber(int to = int.MaxValue) => RandomNumber(0, to);

        private static int RandomNumber(int from,  int to)
        {
            lock (SyncLock)
            {
                return Random.Next(from, to);
            }
        }
    }
}