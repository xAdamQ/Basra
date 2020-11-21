using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basra.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IConfiguration _configuration { get; }
        public IWebHostEnvironment _webHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MasterContext>(options =>
            {
                options.UseMySQL(_configuration.GetConnectionString("Main"));
            });


            services.AddIdentityCore<BasraIdentityUser>()//this is responsible for UserManager, SignInManger registeration
            .AddSignInManager<SignInManager<BasraIdentityUser>>()
            .AddUserManager<UserManager<BasraIdentityUser>>()
            .AddEntityFrameworkStores<MasterContext>();

            services.AddScoped<FbigSecurityManager>();

            services.AddAuthentication(FbigAuthenticationHandler.PROVIDER_NAME)
            .AddScheme<FbigAuthenticationSchemeOptions, FbigAuthenticationHandler>(FbigAuthenticationHandler.PROVIDER_NAME, null);
            //is it ok to make the scheme naem and provider name the same?

            services.AddCors();
            services.AddControllers();
            services.AddSignalR(options =>
            {
                // options.
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
