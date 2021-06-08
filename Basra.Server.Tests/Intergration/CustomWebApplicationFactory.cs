namespace Basra.Server.Tests.Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CustomWebApplicationFactory(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Register the xUnit logger
            builder.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.Services.AddSingleton<ILoggerProvider>(serviceProvider => new XUnitLoggerProvider(_testOutputHelper));
            });
        }
    }
}