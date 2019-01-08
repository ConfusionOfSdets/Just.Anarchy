using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Just.ContainedAnarchy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging
                        .ClearProviders()
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("Just.Anarchy", LogLevel.Debug)
                        .AddConsole();
                })
                .UseStartup<Startup>();
    }
}
