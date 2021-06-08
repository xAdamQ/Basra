using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Basra.Server.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
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
            //identity and mysql config
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

            // services.AddDbContextPool<MasterContext>(options =>
            // {
            //     options.UseMySql
            //     (
            //         _configuration.GetConnectionString("Main"),
            //         new MySqlServerVersion(new Version(8, 0, 21)),
            //         mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
            //     );
            // });

            services.AddDbContext<MasterContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("Main")));
            // services.AddHostedService<ServerLoop>();

            services.AddScoped<IMasterRepo, MasterRepo>();
            services.AddScoped<IRoomManager, RoomManager>();
            services.AddScoped<ILobbyManager, LobbyManager>();
            services.AddScoped<IRequestCache, RequestCache>();
            services.AddScoped<IMatchMaker, MatchMaker>();

            services.AddSingleton<ISessionRepo, SessionRepo>();
            services.AddSingleton<IServerLoop, ServerLoop>();
            services.AddSingleton(new MasterHub.MethodDomains());

            // services.AddDbContext<MasterContext>(options =>
            // {
            //     options.UseMySQL(_configuration.GetConnectionString("Main"));
            // });


            //services.AddIdentityCore<Identity.User>()//this is responsible for UserManager, SignInManger registeration
            //.AddSignInManager<SignInManager<Identity.User>>()
            //.AddUserManager<UserManager<Identity.User>>()
            //.AddEntityFrameworkStores<Identity.IdentityConetxt>();

            services.AddScoped<FbigSecurityManager>();

            services.AddHttpContextAccessor();

            services.AddAuthentication(options =>
            {
                options.AddScheme<FbigAuthenticationHandler>(FbigAuthenticationHandler.PROVIDER_NAME,
                    FbigAuthenticationHandler.PROVIDER_NAME);
                options.DefaultScheme = FbigAuthenticationHandler.PROVIDER_NAME;
            });

            services.AddCors();
            services.AddControllers();
            services.AddSignalR(options =>
            {
                options.AddFilter<BadUserInputFilter>();
                options.ClientTimeoutInterval = TimeSpan.FromHours(1); //change this in production
            });


            services.AddHangfire(configuration =>
            {
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseMemoryStorage()
                    .WithJobExpirationTimeout(TimeSpan.FromHours(3))
                    .UseDefaultTypeSerializer();
            });

            services.AddHangfireServer();
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

            // app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseEndpoints(endpoint => endpoint.MapHub<MasterHub>("connect"));


            // //tododone check if this is needed
            // masterRepo.MarkAllUsersNotActive();
            // masterRepo.SaveChanges();
        }
    }
}