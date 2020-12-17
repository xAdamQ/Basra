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
    /// radnom in multithreads
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

    public enum someVType { vlaue11, vlaue22, value33, ooo }

    public class Program
    {
        public static IHubContext<MasterHub> HubContext;

        // private class tstCla
        // {
        //     public someVType v;

        //     public tstCla()
        //     {
        //         v = someVType.value33;
        //         var saved = new Dictionary<string, object>();
        //         saved.Add(nameof(v), v);
        //         v = someVType.vlaue22;

        //         void someFun(ref someVType vrqq)
        //         {
        //             var a = nameof(vrqq);
        //         }

        //         someFun(ref v);

        //         foreach (var kvp in saved)
        //         {
        //             GetType().GetField(kvp.Key).SetValue(this, kvp.Value);
        //         }

        //         var qq = (object)this;
        //         qq.GetType().GetField("v").SetValue(qq, someVType.ooo);

        //         Console.ReadLine();
        //     }
        // }

        public static void Main(string[] args)
        {
            // new tstCla();

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
