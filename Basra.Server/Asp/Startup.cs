using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Basra.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Basra.Server.Identity;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<MasterContext>(options =>
            {
                options.UseMySql
                (
                    _configuration.GetConnectionString("Main"),
                    new MySqlServerVersion(new Version(8, 0, 21)),
                    mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
                );
                // .EnableSensitiveDataLogging()
                // .EnableDetailedErrors();
            });

            // services.AddDbContext<MasterContext>(options =>
            // {
            //     options.UseMySQL(_configuration.GetConnectionString("Main"));
            // });


            services.AddIdentityCore<BasraIdentityUser>()//this is responsible for UserManager, SignInManger registeration
            .AddSignInManager<SignInManager<BasraIdentityUser>>()
            .AddUserManager<UserManager<BasraIdentityUser>>()
            .AddEntityFrameworkStores<MasterContext>();

            services.AddScoped<FbigSecurityManager>();

            services.AddAuthentication(FbigAuthenticationHandler.PROVIDER_NAME)
            .AddScheme<FbigAuthenticationSchemeOptions, FbigAuthenticationHandler>(FbigAuthenticationHandler.PROVIDER_NAME, null);
            //is it ok to make the scheme name and provider name the same?

            services.AddCors();
            services.AddControllers();
            services.AddSignalR(options =>
            {
                options.AddFilter<BadUserInputFilter>();
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder
           .AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod()
           );

            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }


            // app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoint => endpoint.MapHub<MasterHub>("connect"));
        }
    }
}
