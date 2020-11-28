using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basra.Server
{
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
