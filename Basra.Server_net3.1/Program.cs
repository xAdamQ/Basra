using System;
using System.Collections.Generic;
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
        //working because the type is registered as singleton
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
