using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basra.Server
{

    /// <summary>
    /// random in multithreads
    /// I don't understand the impl, it should do the job
    /// </summary>
    public static class StaticRandom
    {
        private static int Seed = Environment.TickCount;

        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref Seed)));

        public static int GetRandom(int min, int max)
        {
            return Random.Value.Next(min, max);
        }
        public static int GetRandom(int max)
        {
            return Random.Value.Next(max);
        }
    }

    public class Program
    {
        public static IHubContext<MasterHub> HubContext;

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            HubContext = host.Services.GetService(typeof(IHubContext<MasterHub>)) as IHubContext<MasterHub>;

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
