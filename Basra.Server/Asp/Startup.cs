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
            // services.AddDbContextPool<Identity.IdentityConetxt>(options =>
            //{
            //    options.UseMySql
            //    (
            //        _configuration.GetConnectionString("Main"),
            //        new MySqlServerVersion(new Version(8, 0, 21)),
            //        mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
            //    );
            //    // .EnableSensitiveDataLogging()
            //    // .EnableDetailedErrors();
            //});
            services.AddDbContextPool<MasterContext>(options =>
            {
                options.UseMySql
                (
                    _configuration.GetConnectionString("Main"),
                    new MySqlServerVersion(new Version(8, 0, 21)),
                    mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
                );
            });


            services.AddScoped<IMasterRepo, MasterRepo>();

            // services.AddDbContext<MasterContext>(options =>
            // {
            //     options.UseMySQL(_configuration.GetConnectionString("Main"));
            // });

            services.AddHostedService<ServerLoop>();

            //services.AddIdentityCore<Identity.User>()//this is responsible for UserManager, SignInManger registeration
            //.AddSignInManager<SignInManager<Identity.User>>()
            //.AddUserManager<UserManager<Identity.User>>()
            //.AddEntityFrameworkStores<Identity.IdentityConetxt>();

            services.AddScoped<FbigSecurityManager>();

            services.AddAuthentication(FbigAuthenticationHandler.PROVIDER_NAME)
                .AddScheme<FbigAuthenticationSchemeOptions, FbigAuthenticationHandler>(
                    FbigAuthenticationHandler.PROVIDER_NAME, null);
            //is it ok to make the scheme name and provider name the same?

            services.AddCors();
            services.AddControllers();
            services.AddSignalR(options => { options.AddFilter<BadUserInputFilter>(); });

            services.AddScoped<IRoomManager, RoomManager>();
            // services.AddSingleton<RoomFactory>();
            //the factory seems useles in terms of AskForRoom(), made to make us able to change the MakeRoom impl, but it's not even an interface yet
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMasterRepo masterRepo)
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

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseEndpoints(endpoint => endpoint.MapHub<MasterHub>("connect"));

            masterRepo.MarkAllUsersNotActive();
            masterRepo.SaveChanges();
        }
    }
}